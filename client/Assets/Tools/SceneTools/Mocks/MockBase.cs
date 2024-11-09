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

        private void OnApplicationQuit()
        {
            _childScopeLoadResult?.Lifetime.Terminate();
            _serviceScopeLoadResult?.Lifetime.Terminate();
        }
    }
}