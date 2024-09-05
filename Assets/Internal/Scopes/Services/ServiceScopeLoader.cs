using System;
using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;
using Object = UnityEngine.Object;

namespace Internal
{
    public class ServiceScopeLoader : IServiceScopeLoader
    {
        public ServiceScopeLoader(IAssetEnvironment assets, ISceneLoader sceneLoader)
        {
            _assets = assets;
            _sceneLoader = sceneLoader;
        }

        private readonly IAssetEnvironment _assets;
        private readonly ISceneLoader _sceneLoader;

        public IAssetEnvironment Assets => _assets;

        public async UniTask<IServiceScopeLoadResult> Load(
            LifetimeScope parent,
            ServiceScopeData data,
            Func<IScopeBuilder, UniTask> construct)
        {
            var profiler = new ProfilingScope("ServiceScopeLoader");

            profiler.Step("Start");
            var sceneLoader = new ServiceScopeSceneLoader(_sceneLoader);
            profiler.Step("Scene loader created");

            var builder = await CreateBuilder(sceneLoader, data, profiler);

            await construct.Invoke(builder);
            profiler.Step("Construct callback invoked");
            await BuildContainer(builder, parent, profiler);

            var eventLoop = builder.Scope.Container.Resolve<IEventLoop>();
            profiler.Step("Run event loop construct");
            await eventLoop.RunConstruct(builder.Lifetime);

            var loadResult = new ScopeLoadResult(
                builder.Scope,
                builder.Lifetime,
                eventLoop,
                sceneLoader);

            profiler.Dispose();

            return loadResult;
        }

        public async UniTask<IServiceScopeLoadResult> Load(LifetimeScope parent, IServiceScopeConfig config)
        {
            var profiler = new ProfilingScope("ServiceScopeLoader");

            var sceneLoader = new ServiceScopeSceneLoader(_sceneLoader);
            var builder = await CreateBuilder(sceneLoader, config.GetData(_assets), profiler);
            await config.Construct(builder);
            await BuildContainer(builder, parent, profiler);

            var eventLoop = builder.Scope.Container.Resolve<IEventLoop>();
            await eventLoop.RunConstruct(builder.Lifetime);

            var loadResult = new ScopeLoadResult(
                builder.Scope,
                builder.Lifetime,
                eventLoop,
                sceneLoader);

            profiler.Dispose();

            return loadResult;
        }

        private async UniTask<ScopeBuilder> CreateBuilder(
            ISceneLoader sceneLoader,
            ServiceScopeData scopeData, 
            ProfilingScope profiler)
        {
            profiler.Step("CreateBuilder started");
            var servicesScene = await sceneLoader.Load(scopeData.ServicesScene);
            profiler.Step("Scene services loaded");
            var binder = new ServiceScopeBinder(servicesScene.Scene);
            var scope = Object.Instantiate(scopeData.ScopePrefab);
            profiler.Step("Scope instantiated");
            binder.MoveToModules(scope.gameObject);
            var lifetime = new Lifetime();
            var services = new ServiceCollection();

            profiler.Step("CreateBuilder completed");
            return new ScopeBuilder(services, _assets, sceneLoader, binder, scope, lifetime, scopeData.IsMock);
        }

        private async UniTask BuildContainer(
            ScopeBuilder builder,
            LifetimeScope parent,
            ProfilingScope profiler)
        {
            var scope = builder.Scope;
            profiler.Step("BuildContainer started");
            using (LifetimeScope.EnqueueParent(parent))
            {
                using (LifetimeScope.Enqueue(Register))
                {
                    profiler.Step("BuildContainer scope.Build");
                    scope.Build();
                }
            }

            profiler.Step("Container built");
            builder.ServicesInternal.Resolve(scope.Container);
            profiler.Step("BuildContainer completed");
            return;

            void Register(IContainerBuilder container)
            {
                container.Register<IEventLoop, EventLoop>(VContainer.Lifetime.Scoped);
                container.Register<IViewInjector, ViewInjector>(VContainer.Lifetime.Scoped);
                
                builder.ServicesInternal.PassRegistrations(container);
            }
        }
    }
}