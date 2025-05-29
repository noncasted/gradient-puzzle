using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GamePlay.Common;
using GamePlay.Levels;
using GamePlay.Paints;
using GamePlay.Paints.Collection;
using GamePlay.Selections;
using Global.Backend;
using Global.Cameras;
using Global.GameServices;
using Global.Metrics;
using Global.Publisher;
using Global.Saves;
using Global.Systems;
using Global.UI;
using Internal;
using Menu;
using Menu.Completion;
using Services;
using Shared;
using UnityEngine;

namespace Loop
{
    public class GameLoop : IGameLoop
    {
        public GameLoop(
            IUIStateMachine stateMachine,
            IMainOverlay overlay,
            IGameCamera gameCamera,
            ICurrentCameraProvider cameraProvider,
            ILevelsStorage levelsStorage,
            ILevelLoader levelLoader,
            IPaintFactory paintFactory,
            IPaintSelection selection,
            IPaintDragStarter dragStarter,
            IPaintSelectionScaler selectionScaler,
            ICompletionUI completionUI,
            IUpdater updater,
            IPaintCollection paintCollection,
            IGameContext gameContext,
            IGlobalContext globalContext,
            IUserBackend userBackend,
            IDataStorage dataStorage,
            IBackground background,
            IMetrics metrics,
            GameLoopCheats cheats)
        {
            _stateMachine = stateMachine;
            _overlay = overlay;
            _gameCamera = gameCamera;
            _cameraProvider = cameraProvider;
            _levelsStorage = levelsStorage;
            _levelLoader = levelLoader;
            _paintFactory = paintFactory;
            _selection = selection;
            _dragStarter = dragStarter;
            _selectionScaler = selectionScaler;
            _completionUI = completionUI;
            _updater = updater;
            _paintCollection = paintCollection;
            _gameContext = gameContext;
            _globalContext = globalContext;
            _userBackend = userBackend;
            _dataStorage = dataStorage;
            _background = background;
            _metrics = metrics;
            _cheats = cheats;
        }

        private const float PointSpawnDelay = 0.1f;

        private readonly IUIStateMachine _stateMachine;
        private readonly IMainOverlay _overlay;
        private readonly IGameCamera _gameCamera;
        private readonly ICurrentCameraProvider _cameraProvider;
        private readonly ILevelsStorage _levelsStorage;
        private readonly ILevelLoader _levelLoader;
        private readonly IPaintFactory _paintFactory;
        private readonly IPaintSelection _selection;
        private readonly IPaintDragStarter _dragStarter;
        private readonly IPaintSelectionScaler _selectionScaler;
        private readonly ICompletionUI _completionUI;
        private readonly IUpdater _updater;
        private readonly IPaintCollection _paintCollection;
        private readonly IGameContext _gameContext;
        private readonly IGlobalContext _globalContext;
        private readonly IUserBackend _userBackend;
        private readonly IDataStorage _dataStorage;
        private readonly IBackground _background;
        private readonly IMetrics _metrics;
        private readonly GameLoopCheats _cheats;

        private ILifetime _currentLifetime;
        private ILevelData _currentSelection;
        private IReadOnlyLifetime _parentLifetime;

        public async UniTask Process(IReadOnlyLifetime lifetime)
        {
            Debug.Log("Start game loop");
            await _globalContext.Init(lifetime);

            Debug.Log("Get progress from backend");
            var progress = await _userBackend.GetProgress(lifetime);
            var levelSave = _dataStorage.Get<LevelsSave>();

            foreach (var (sectionKey, index) in progress)
            {
                Debug.Log($"Progress received: {sectionKey} {index}");
                levelSave.Passed[sectionKey] = index;
            }

            await _dataStorage.Save(levelSave);

            await _levelsStorage.RecalculateUnlocks();

            _parentLifetime = lifetime;
            _cameraProvider.SetCamera(_gameCamera.Camera);

            _stateMachine.EnterChild(_stateMachine.Base, _overlay);

            _overlay.LevelSelected.Advise(lifetime, LoadLevel);
            _overlay.ResetClicked.Advise(lifetime, () => LoadLevel(_currentSelection));

            _overlay.ShowSections().Forget();

            //  LoadLevel(_levelsStorage.Get(LevelSectionType.Basic, 0));
        }

        private void LoadLevel(ILevelData data)
        {
            _currentLifetime?.Terminate();
            _currentLifetime = _parentLifetime.Child();
            _currentSelection = data;
            HandleLevel(_currentLifetime, _currentSelection).Forget();
        }

        private async UniTask HandleLevel(IReadOnlyLifetime lifetime, ILevelData data)
        {
            var startTime = DateTime.Now;
            _overlay.HideReset();

            await _paintCollection.Initialize();

            var level = _levelLoader.Load(data);

            var colors = new List<Color>();
            var positionToColor = new Dictionary<Vector2, Color>();
            var colorToPaint = new Dictionary<Color, IPaint>();
            var paintToColor = new Dictionary<IPaint, Color>();
            var paintToDock = new Dictionary<IPaint, IPaintDock>();
            var colorToArea = new Dictionary<Color, IArea>();
            var target = new List<IPaintTarget>();
            var docks = new List<IPaintDock>();

            foreach (var area in level.Areas)
            {
                colors.Add(area.Color);
                positionToColor.Add(area.RootCenter.anchoredPosition, area.Color);
                target.Add(area);
                colorToArea.Add(area.Color, area);
            }

            _selection.Clear();
            _selectionScaler.Scale(colors.Count);

            colors.Shuffle();

            var paints = new List<IPaint>();

            foreach (var color in colors)
            {
                var paint = await _paintFactory.Create(lifetime, color);
                paints.Add(paint);
                colorToPaint.Add(color, paint);
                paintToColor.Add(paint, color);
            }

            foreach (var paint in paints)
            {
                var color = paintToColor[paint];
                paint.Construct(color);
                var dock = _selection.CreateDock();
                docks.Add(dock);
                target.Add(dock);
                paintToDock.Add(paint, dock);
            }

            _gameContext.Setup(level, docks, data);
            _background.SetGame(positionToColor);
            _background.ToGame();

            await UniTask.Yield();

            _selectionScaler.Disable();

            foreach (var dock in docks)
                dock.UpdateTransform(_selectionScaler.AreaSize);

            paints.Shuffle();

            foreach (var paint in paints)
            {
                var dock = paintToDock[paint];
                paint.Spawn(dock).Forget();
                await UniTask.Delay(TimeSpan.FromSeconds(PointSpawnDelay));
            }

            var anchors = level.Areas.Where(t => t.IsAnchor);

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            foreach (var anchor in anchors)
            {
                var paint = colorToPaint[anchor.Color];
                anchor.PaintHandle.Lock();
                paint.Anchor(anchor).Forget();
            }

            var levelLifetime = lifetime.Child();

            level.Setup(levelLifetime);
            _dragStarter.Start(levelLifetime, target);
            _overlay.ShowReset();

            _cheats.Complete.Advise(lifetime, async () =>
            {
                var tasks = new List<UniTask>();

                foreach (var paint in paints)
                {
                    var paintTarget = colorToArea[paint.Color];
                    tasks.Add(paint.Anchor(paintTarget));
                }

                await UniTask.WhenAll(tasks);
            });

            var completionAwaiter = new GameCompletionAwaiter(level.Areas, lifetime);
            await completionAwaiter.Await();

            _overlay.HideReset();
            levelLifetime.Terminate();

            var completionTasks = new List<UniTask>();

            var completionOrderedAreas = level.Areas.OrderBy(t => t.Position.y);
            await UniTask.Delay(TimeSpan.FromSeconds(1f));

            foreach (var area in completionOrderedAreas)
            {
                completionTasks.Add(area.GetPaint().Complete());
                await UniTask.Delay(TimeSpan.FromSeconds(0.05f));
            }

            await UniTask.WhenAll(completionTasks);
            await UniTask.Delay(TimeSpan.FromSeconds(1f));

            _levelsStorage.OnLevelPassed(data);
            
            var resultTime = DateTime.Now - startTime;

            _metrics.Send(new MetricsContexts.LevelPass()
            {
                Section = data.Options.SectionType,
                LevelIndex = data.Index,
                Time = resultTime
            }).Forget();

            await _stateMachine.ProcessChild(_overlay, _completionUI);

            LoadLevel(_levelsStorage.GetNext(data));
        }
    }
}