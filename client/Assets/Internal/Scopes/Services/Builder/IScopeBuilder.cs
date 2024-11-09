using VContainer.Unity;

namespace Internal
{
    public interface IScopeBuilder
    {
        IServiceCollection Services { get; }
        IAssetEnvironment Assets { get; }
        ISceneLoader SceneLoader { get; }
        IServiceScopeBinder Binder { get; }
        IScopeEventListeners EventListeners { get; }
        LifetimeScope Scope { get; }
        ILifetime Lifetime { get; }
        bool IsMock { get; }
    }
}