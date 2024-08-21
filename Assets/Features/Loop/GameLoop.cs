using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Features.GamePlay;
using Features.Services;
using Global.Cameras;
using Internal;
using UnityEngine;

namespace Features.Loop
{
    public class GameLoop : IGameLoop
    {
        public GameLoop(
            IGameCamera gameCamera,
            ICurrentCameraProvider cameraProvider,
            ILevelsStorage levelsStorage,
            ILevelLoader levelLoader,
            IPaintFactory paintFactory,
            IPaintSelection selection,
            IPaintMover mover,
            IPaintSelectionScaler selectionScaler)
        {
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

        private readonly IGameCamera _gameCamera;
        private readonly ICurrentCameraProvider _cameraProvider;
        private readonly ILevelsStorage _levelsStorage;
        private readonly ILevelLoader _levelLoader;
        private readonly IPaintFactory _paintFactory;
        private readonly IPaintSelection _selection;
        private readonly IPaintMover _mover;
        private readonly IPaintSelectionScaler _selectionScaler;

        public async UniTask Process(IReadOnlyLifetime lifetime)
        {
            _cameraProvider.SetCamera(_gameCamera.Camera);

            var levelConfiguration = _levelsStorage.Get(0);
            var level = _levelLoader.Load(levelConfiguration);

            var colors = new List<Color>();
            var colorToArea = new Dictionary<Color, IArea>();
            var paintToColor = new Dictionary<IPaint, Color>();
            var paintToDock = new Dictionary<IPaint, IPaintDock>();
            var target = new List<IPaintTarget>();
            var docks = new List<IPaintDock>();

            foreach (var area in level.Areas)
            {
                colors.Add(area.Source);
                colorToArea.Add(area.Source, area);
                target.Add(area);
            }

            _selectionScaler.Scale(colors.Count);

            colors.Shuffle();

            var paints = new List<IPaint>();

            foreach (var color in colors)
            {
                var paint = await _paintFactory.Create(lifetime, color);
                paints.Add(paint);
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
            
            _selectionScaler.Disable();

            await UniTask.Yield();
            
            foreach (var dock in docks)
                dock.UpdateTransform(_selectionScaler.AreaSize);

            level.Setup(lifetime);
            _mover.Start(lifetime, target);
        }
    }
}