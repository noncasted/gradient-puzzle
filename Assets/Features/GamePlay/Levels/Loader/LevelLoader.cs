using Features.Services;
using Internal;

namespace Features.GamePlay
{
    public class LevelLoader : ILevelLoader
    {
        public LevelLoader(IObjectFactory<Level> objectFactory, LevelLoaderOptions options)
        {
            _objectFactory = objectFactory;
            _options = options;
        }
        
        private readonly IObjectFactory<Level> _objectFactory;
        private readonly LevelLoaderOptions _options;

        public ILevel Load(ILevelConfiguration configuration)
        {
            var level = _objectFactory.Create(configuration.Prefab);
            
            foreach (var area in level.Areas)
                area.Construct(_options.AreasGradient.Evaluate(RandomExtensions.RandomOne()));
            
            return level;
        }
    }
}