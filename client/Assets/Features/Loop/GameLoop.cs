using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GamePlay.Common;
using GamePlay.Levels;
using GamePlay.Paints;
using GamePlay.Selections;
using Global.Cameras;
using Global.Systems;
using Global.UI;
using Internal;
using Overlay;
using Services;
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
            _cheats = cheats;
        }

        private const float PointSpawnDelay = 0.05f;

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
        private readonly GameLoopCheats _cheats;

        private ILifetime _currentLifetime;
        private ILevelConfiguration _currentSelection;
        private IReadOnlyLifetime _parentLifetime;

        public UniTask Process(IReadOnlyLifetime lifetime)
        {
            _parentLifetime = lifetime;
            _cameraProvider.SetCamera(_gameCamera.Camera);

            _stateMachine.EnterChild(_stateMachine.Base, _overlay);

            _overlay.LevelSelected.Advise(lifetime, LoadLevel);
            _overlay.ResetClicked.Advise(lifetime, () => LoadLevel(_currentSelection));

            LoadLevel(_levelsStorage.Get(0));

            return UniTask.CompletedTask;
        }

        private void LoadLevel(ILevelConfiguration configuration)
        {
            _currentLifetime?.Terminate();
            _currentLifetime = _parentLifetime.Child();
            _currentSelection = configuration;
            HandleLevel(_currentLifetime, _currentSelection).Forget();
        }

        private async UniTask HandleLevel(IReadOnlyLifetime lifetime, ILevelConfiguration configuration)
        {
            _overlay.HideReset();
            var level = _levelLoader.Load(configuration);

            var colors = new List<Color>();
            var colorToPaint = new Dictionary<Color, IPaint>();
            var paintToColor = new Dictionary<IPaint, Color>();
            var paintToDock = new Dictionary<IPaint, IPaintDock>();
            var colorToArea = new Dictionary<Color, IArea>();
            var target = new List<IPaintTarget>();
            var docks = new List<IPaintDock>();

            foreach (var area in level.Areas)
            {
                colors.Add(area.Source);
                target.Add(area);
                colorToArea.Add(area.Source, area);
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

            paints.Shuffle();

            foreach (var paint in paints)
            {
                var dock = paintToDock[paint];
                paint.Spawn(dock);
                await UniTask.Delay(TimeSpan.FromSeconds(PointSpawnDelay));
            }

            var anchors = level.Areas.Where(t => t.IsAnchor);

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            foreach (var anchor in anchors)
            {
                var paint = colorToPaint[anchor.Source];
                anchor.PaintHandle.Set(paint);
                paint.Anchor(anchor);
            }

            _selectionScaler.Disable();

            await UniTask.Yield();

            foreach (var dock in docks)
                dock.UpdateTransform(_selectionScaler.AreaSize);

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
            await UniTask.Delay(TimeSpan.FromSeconds(2f));

            _levelsStorage.OnLevelPassed(configuration);

            await _stateMachine.ProcessChild(_overlay, _completionUI);

            LoadLevel(_levelsStorage.GetNext(configuration));
        }
    }
}