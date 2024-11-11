using Common.Setup;
using Cysharp.Threading.Tasks;
using Global.Cameras;
using Global.GameLoops;
using Global.UI;
using Internal;

namespace Loop
{
    public class GamePlayLoader : IGamePlayLoader
    {
        public GamePlayLoader(
            IServiceScopeLoader scopeLoaderFactory,
            ILoadingScreen loadingScreen,
            IGlobalCamera globalCamera,
            ICurrentCameraProvider currentCameraProvider)
        {
            _scopeLoaderFactory = scopeLoaderFactory;
            _loadingScreen = loadingScreen;
            _globalCamera = globalCamera;
            _currentCameraProvider = currentCameraProvider;
        }

        private readonly IServiceScopeLoader _scopeLoaderFactory;
        private readonly ILoadingScreen _loadingScreen;
        private readonly IGlobalCamera _globalCamera;
        private readonly ICurrentCameraProvider _currentCameraProvider;

        private ILoadedScope _currentScope;

        public UniTask Initialize(ILoadedScope parent)
        {
            return LoadGamePlay(parent);
        }

        private async UniTask LoadGamePlay(ILoadedScope parent)
        {
            _globalCamera.Enable();
            _currentCameraProvider.SetCamera(_globalCamera.Camera);

            var unloadTask = UniTask.CompletedTask;

            if (_currentScope != null)
                unloadTask = _currentScope.Dispose();

            var loadResult = await _scopeLoaderFactory.ProcessGamePlay(parent);

            await unloadTask;
            _currentScope = loadResult;
            _globalCamera.Disable();
        }
    }
}