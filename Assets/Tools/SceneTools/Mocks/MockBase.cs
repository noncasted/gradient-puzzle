using Cysharp.Threading.Tasks;
using Global.Setup;
using Global.UI;
using Internal;
using UnityEngine;
using VContainer;

namespace Tools
{
    [DisallowMultipleComponent]
    public abstract class MockBase : MonoBehaviour
    {
        private IServiceScopeLoadResult _serviceScopeLoadResult;
        private IServiceScopeLoadResult _childScopeLoadResult;

        public abstract UniTaskVoid Process();

        protected async UniTask<IServiceScopeLoadResult> BootstrapGlobal()
        {
            var internalConfig = AssetsExtensions.Environment.GetAsset<InternalScopeConfig>();
            var internalScopeLoader = new InternalScopeLoader(internalConfig);
            var internalScope = await internalScopeLoader.Load();
            var scopeLoader = internalScope.Container.Resolve<IServiceScopeLoader>();
            
            var scopeLoadResult = await scopeLoader.LoadGlobalMock(internalScope);
            await scopeLoadResult.EventLoop.RunLoaded(scopeLoadResult.Lifetime);
            
            _serviceScopeLoadResult = scopeLoadResult;
            _serviceScopeLoadResult.Scope.Container.Resolve<ILoadingScreen>().HideGameLoading();

            return _serviceScopeLoadResult;
        }

        protected async UniTask<IServiceScopeLoadResult> LoadChildScope(IServiceScopeConfig config)
        {
            _serviceScopeLoadResult ??= await BootstrapGlobal();

            var parentScope = _serviceScopeLoadResult.Scope;
            var scopeLoaderFactory = parentScope.Container.Resolve<IServiceScopeLoader>();
            var childLoadResult = await scopeLoaderFactory.Load(parentScope, config);
            await childLoadResult.EventLoop.RunLoaded(childLoadResult.Lifetime);

            _childScopeLoadResult = childLoadResult;

            return childLoadResult;
        }

        private void OnApplicationQuit()
        {
            _childScopeLoadResult?.Lifetime.Terminate();
            _serviceScopeLoadResult?.Lifetime.Terminate();
        }
    }
}