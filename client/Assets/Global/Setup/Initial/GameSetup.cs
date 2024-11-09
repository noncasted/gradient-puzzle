using Cysharp.Threading.Tasks;
using Internal;
using UnityEngine;
using VContainer;

namespace Global.Setup
{
    [DisallowMultipleComponent]
    public class GameSetup : MonoBehaviour
    {
        [SerializeField] private InternalScopeConfig _internal;
        [SerializeField] private SetupLoadingScreen _loading;

        private void Awake()
        {
            Setup().Forget();
        }

        private async UniTask Setup()
        {
            var profiler = new ProfilingScope("GameSetup");
            var internalScopeLoader = new InternalScopeLoader(_internal);
            
            var internalScope = await internalScopeLoader.Load();
            
            var scopeLoader = internalScope.Container.Resolve<IServiceScopeLoader>();
            var globalLoadResult = await scopeLoader.LoadGlobal(internalScope);
            
            await globalLoadResult.EventLoop.RunLoaded(globalLoadResult.Lifetime);

            _loading.Dispose();
            profiler.Dispose();
        }
    }
}