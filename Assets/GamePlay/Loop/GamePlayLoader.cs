using Cysharp.Threading.Tasks;
using GamePlay.Setup;
using Global.Cameras;
using Global.Systems;
using Global.UI;
using Internal;
using VContainer.Unity;

namespace Loop
{
    public class GamePlayLoader : IScopeLoadedAsync
    {
        public GamePlayLoader(
            LifetimeScope parent,
            IServiceScopeLoader scopeLoaderFactory,
            ILoadingScreen loadingScreen,
            IGlobalCamera globalCamera,
            IScopeDisposer scopeDisposer,
            ICurrentCameraProvider currentCameraProvider)
        {
            _parent = parent;
            _scopeLoaderFactory = scopeLoaderFactory;
            _loadingScreen = loadingScreen;
            _globalCamera = globalCamera;
            _scopeDisposer = scopeDisposer;
            _currentCameraProvider = currentCameraProvider;
        }

        private readonly LifetimeScope _parent;
        private readonly IServiceScopeLoader _scopeLoaderFactory;
        private readonly ILoadingScreen _loadingScreen;
        private readonly IGlobalCamera _globalCamera;
        private readonly IScopeDisposer _scopeDisposer;
        private readonly ICurrentCameraProvider _currentCameraProvider;

        private IServiceScopeLoadResult _currentScope;

        public async UniTask OnLoadedAsync(IReadOnlyLifetime lifetime)
        {
            Process(lifetime).Forget();
        }

        private async UniTaskVoid Process(IReadOnlyLifetime lifetime)
        {
            await LoadGamePlay(lifetime);
        }

        private async UniTask LoadGamePlay(IReadOnlyLifetime lifetime)
        {
            _globalCamera.Enable();
            _currentCameraProvider.SetCamera(_globalCamera.Camera);

            var unloadTask = UniTask.CompletedTask;

            if (_currentScope != null)
                unloadTask = _scopeDisposer.Unload(_currentScope);

            var loadResult = await _scopeLoaderFactory.ProcessGamePlay(_parent);

            await unloadTask;
            _currentScope = loadResult;
            _globalCamera.Disable();
        }
    }
}