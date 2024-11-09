using System.Collections.Generic;
using VContainer.Unity;

namespace Internal
{
    public class ScopeLoadResult : IServiceScopeLoadResult
    {
        public ScopeLoadResult(
            LifetimeScope scope,
            ILifetime lifetime,
            IEventLoop eventLoop,
            ServiceScopeSceneLoader sceneLoader)
        {
            Scope = scope;
            Lifetime = lifetime;
            EventLoop = eventLoop;
            SceneLoader = sceneLoader;
            Scenes = sceneLoader.Results;
        }

        public LifetimeScope Scope { get; }
        public ILifetime Lifetime { get; }
        public IEventLoop EventLoop { get; }
        public ServiceScopeSceneLoader SceneLoader { get; }
        public IReadOnlyList<ISceneLoadResult> Scenes { get; }
    }
}