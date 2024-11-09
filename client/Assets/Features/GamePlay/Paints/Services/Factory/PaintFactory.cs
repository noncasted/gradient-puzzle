using Cysharp.Threading.Tasks;
using Internal;
using Services;
using UnityEngine;
using VContainer.Unity;

namespace GamePlay.Paints
{
    public class PaintFactory : IPaintFactory
    {
        public PaintFactory(
            IObjectFactory<PaintView> objectFactory,
            IEntityScopeLoader entityScopeLoader,
            LifetimeScope parent,
            PaintFactoryOptions options)
        {
            _objectFactory = objectFactory;
            _entityScopeLoader = entityScopeLoader;
            _parent = parent;
            _options = options;
        }


        private readonly IObjectFactory<PaintView> _objectFactory;
        private readonly IEntityScopeLoader _entityScopeLoader;
        private readonly LifetimeScope _parent;
        private readonly PaintFactoryOptions _options;

        public async UniTask<IPaint> Create(IReadOnlyLifetime lifetime, Color color)
        {
            var view = _objectFactory.Create(_options.Prefab);
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

                builder.Register<Paint>()
                    .As<IPaint>();
            }
        }
    }
}