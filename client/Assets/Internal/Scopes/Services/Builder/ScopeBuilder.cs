using VContainer.Unity;

namespace Internal
{
    public class ScopeBuilder : IScopeBuilder
    {
        public ScopeBuilder(
            ServiceCollection services,
            IAssetEnvironment assets,
            ISceneLoader sceneLoader,
            IServiceScopeBinder binder,
            LifetimeScope scope,
            ILifetime lifetime,
            bool isMock)
        {
            Services = services;
            ServicesInternal = services;
            Assets = assets;
            SceneLoader = sceneLoader;
            Binder = binder;
            Scope = scope;
            Lifetime = lifetime;
            IsMock = isMock;
            EventListeners = new ScopeEventListeners();
        }

        public IServiceCollection Services { get; }
        public IAssetEnvironment Assets { get; }
        public ISceneLoader SceneLoader { get; }
        public IServiceScopeBinder Binder { get; }
        public IScopeEventListeners EventListeners { get; }
        public LifetimeScope Scope { get; }
        public ILifetime Lifetime { get; }
        public bool IsMock { get; }
        
        public ServiceCollection ServicesInternal { get; }
    }
}