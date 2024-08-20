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

            var sceneLoader = new ServiceScopeSceneLoader(_sceneLoader);
            var builder = await CreateBuilder(sceneLoader, data);

            await construct.Invoke(builder);
            await BuildContainer(builder, parent);

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

        public async UniTask<IServiceScopeLoadResult> Load(LifetimeScope parent, IServiceScopeConfig config)
        {
            var profiler = new ProfilingScope("ServiceScopeLoader");

            var sceneLoader = new ServiceScopeSceneLoader(_sceneLoader);
            var builder = await CreateBuilder(sceneLoader, config.GetData(_assets));
            await config.Construct(builder);
            await BuildContainer(builder, parent);

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
            ServiceScopeData scopeData)
        {
            var servicesScene = await sceneLoader.Load(scopeData.ServicesScene);
            var binder = new ServiceScopeBinder(servicesScene.Scene);
            var scope = Object.Instantiate(scopeData.ScopePrefab);
            binder.MoveToModules(scope.gameObject);
            var lifetime = new Lifetime();
            var services = new ServiceCollection();

            return new ScopeBuilder(services, _assets, sceneLoader, binder, scope, lifetime, scopeData.IsMock);
        }

        private async UniTask BuildContainer(
            ScopeBuilder builder,
            LifetimeScope parent)
        {
            var scope = builder.Scope;

            using (LifetimeScope.EnqueueParent(parent))
            {
                using (LifetimeScope.Enqueue(Register))
                {
                    await UniTask.RunOnThreadPool(() => scope.Build());
                }
            }

            builder.ServicesInternal.Resolve(scope.Container);
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