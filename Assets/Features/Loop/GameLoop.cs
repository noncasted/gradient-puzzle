using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Features.GamePlay;
using Features.Services;
using Global.Cameras;
using Global.UI;
using Internal;
using UnityEngine;

namespace Features
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
            IPaintMover mover,
            IPaintSelectionScaler selectionScaler)
        {
            _stateMachine = stateMachine;
            _overlay = overlay;
            _gameCamera = gameCamera;
            _cameraProvider = cameraProvider;
            _levelsStorage = levelsStorage;
            _levelLoader = levelLoader;
            _paintFactory = paintFactory;
            _selection = selection;
            _mover = mover;
            _selectionScaler = selectionScaler;
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
        private readonly IPaintMover _mover;
        private readonly IPaintSelectionScaler _selectionScaler;

        private ILifetime _currentLifetime;
        private ILevelConfiguration _currentSelection;

        public async UniTask Process(IReadOnlyLifetime lifetime)
        {
            _cameraProvider.SetCamera(_gameCamera.Camera);

            _stateMachine.EnterChild(_stateMachine.Base, _overlay);

            _overlay.LevelSelected.Advise(lifetime, levelConfiguration =>
            {
                _currentLifetime?.Terminate();
                _currentLifetime = lifetime.Child();
                _currentSelection = levelConfiguration;
                HandleLevel(_currentLifetime, levelConfiguration).Forget();
            });

            _overlay.ResetClicked.Advise(lifetime, () =>
            {
                _currentLifetime?.Terminate();
                _currentLifetime = lifetime.Child();
                HandleLevel(_currentLifetime, _currentSelection).Forget();
            });

            _currentLifetime = lifetime.Child();
            _currentSelection = _levelsStorage.Get(0);
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
            var target = new List<IPaintTarget>();
            var docks = new List<IPaintDock>();

            foreach (var area in level.Areas)
            {
                colors.Add(area.Source);
                target.Add(area);
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
                dock.SetPaint(paint);
                paint.Spawn(dock);
                await UniTask.Delay(TimeSpan.FromSeconds(PointSpawnDelay));
            }

            var anchors = level.Areas.Where(t => t.IsAnchor);

            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            foreach (var anchor in anchors)
            {
                var paint = colorToPaint[anchor.Source];
                paintToDock[paint].RemovePaint(paint);
                anchor.SetPaint(paint);
                paint.Anchor(anchor);
            }

            _selectionScaler.Disable();

            await UniTask.Yield();

            foreach (var dock in docks)
                dock.UpdateTransform(_selectionScaler.AreaSize);

            level.Setup(lifetime);
            _mover.Start(lifetime, target);

            _overlay.ShowReset();
        }
    }
}