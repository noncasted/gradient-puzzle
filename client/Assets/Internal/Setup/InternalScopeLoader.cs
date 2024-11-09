using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Internal
{
    public class InternalScopeLoader
    {
        public InternalScopeLoader(IInternalScopeConfig config)
        {
            _config = config;
        }

        private readonly IInternalScopeConfig _config;

        public async UniTask<LifetimeScope> Load()
        {
            var profiler = new ProfilingScope("InternalScopeLoader");

            var scope = Object.Instantiate(_config.Scope);

            using (LifetimeScope.Enqueue(Register))
                await UniTask.Create(async () => scope.Build());

            profiler.Dispose();

            return scope;

            void Register(IContainerBuilder container)
            {
                var optionsRegistry = _config.AssetsStorage.Options[_config.Platform];
                optionsRegistry.CacheRegistry();
                optionsRegistry.AddOptions(new PlatformOptions(_config.Platform, Application.isMobilePlatform));
                
                var assets = new AssetEnvironment(_config.AssetsStorage, optionsRegistry);
                var builder = new InternalScopeBuilder(assets, container);

                builder
                    .AddScenes()
                    .AddLogs()
                    .AddScopeLoaders();
                
                container.RegisterInstance(assets)
                    .As<IAssetEnvironment>();
            }
        }
    }
}