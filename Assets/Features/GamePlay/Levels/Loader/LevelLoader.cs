using Features.Services;
using Internal;

namespace Features.GamePlay
{
    public class LevelLoader : ILevelLoader
    {
        public LevelLoader(IObjectFactory<Level> objectFactory, LevelLoaderOptions options, IViewInjector injector)
        {
            _objectFactory = objectFactory;
            _options = options;
            _injector = injector;
        }
        
        private readonly IObjectFactory<Level> _objectFactory;
        private readonly LevelLoaderOptions _options;
        private readonly IViewInjector _injector;

        public ILevel Load(ILevelConfiguration configuration)
        {
            _objectFactory.DestroyAll();
            var level = _objectFactory.Create(configuration.Prefab);

            _injector.Inject(level);
            
            foreach (var area in level.Areas)
                area.Setup(_options.AreasGradient.Evaluate(RandomExtensions.RandomOne()));
            
            return level;
        }
    }
}