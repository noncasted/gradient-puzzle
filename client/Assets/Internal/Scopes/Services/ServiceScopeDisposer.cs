using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Internal
{
    public class ServiceScopeDisposer : IScopeDisposer
    {
        public ServiceScopeDisposer(
            ILifetime lifetime,
            IEventLoop loop,
            IReadOnlyList<ILoadedScene> scenes,
            LifetimeScope container)
        {
            _lifetime = lifetime;
            _loop = loop;
            _scenes = scenes;
            _container = container;
        }

        private readonly ILifetime _lifetime;
        private readonly IEventLoop _loop;
        private readonly IReadOnlyList<ILoadedScene> _scenes;
        private readonly LifetimeScope _container;

        public async UniTask Dispose()
        {
            await _loop.RunDispose();
            _lifetime.Terminate();
            await _scenes.InvokeAsync(scene => scene.Unload());
            _container.Dispose();
        }
    }
}