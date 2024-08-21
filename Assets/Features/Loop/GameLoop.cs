using Cysharp.Threading.Tasks;
using Features.Services.Cameras;
using Global.Cameras;
using Internal;

namespace Features.Loop
{
    public class GameLoop : IGameLoop
    {
        public GameLoop(IGameCamera gameCamera, ICurrentCameraProvider cameraProvider)
        {
            _gameCamera = gameCamera;
            _cameraProvider = cameraProvider;
        }

        private readonly IGameCamera _gameCamera;
        private readonly ICurrentCameraProvider _cameraProvider;

        public async UniTask Process(IReadOnlyLifetime lifetime)
        {
            _cameraProvider.SetCamera(_gameCamera.Camera);
        }
    }
}