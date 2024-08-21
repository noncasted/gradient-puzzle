using Cysharp.Threading.Tasks;
using Internal;
using UnityEngine;
using VContainer.Unity;

namespace Features.GamePlay.Paints
{
    public class PaintFactory : IPaintFactory
    {
        public PaintFactory(
            IEntityScopeLoader entityScopeLoader,
            LifetimeScope parent,
            PaintFactoryOptions options)
        {
            _entityScopeLoader = entityScopeLoader;
            _parent = parent;
            _options = options;
        }


        private readonly IEntityScopeLoader _entityScopeLoader;
        private readonly LifetimeScope _parent;
        private readonly PaintFactoryOptions _options;

        public async UniTask<IPaint> Create(IReadOnlyLifetime lifetime, Color color, Transform parent)
        {
            var view = Object.Instantiate(_options.Prefab, parent);
            view.name = "Paint";
            view.transform.localPosition = Vector3.zero;

            var result = await _entityScopeLoader.Load(lifetime, _parent, view, Construct);
            
            var eventLoop = result.Get<IEventLoop>();
            await eventLoop.RunLoaded(result.Lifetime);

            return result.Get<IPaint>();

            void Construct(IEntityBuilder builder)
            {
                builder
                    .AddComponents()
                    .AddStates();
            }
        }
    }
}