using Internal;
using Services;

namespace GamePlay.Levels
{
    public class LevelLoader : ILevelLoader
    {
        public LevelLoader(
            IObjectFactory<Level> objectFactory,
            LevelLoaderOptions options,
            IViewInjector injector,
            IMaskRenderOptions maskRenderOptions)
        {
            _objectFactory = objectFactory;
            _options = options;
            _injector = injector;
            _maskRenderOptions = maskRenderOptions;
        }

        private readonly IObjectFactory<Level> _objectFactory;
        private readonly LevelLoaderOptions _options;
        private readonly IViewInjector _injector;
        private readonly IMaskRenderOptions _maskRenderOptions;

        public ILevel Load(ILevelConfiguration configuration)
        {
            _objectFactory.DestroyAll();
            var level = _objectFactory.Create(configuration.Prefab);

            _injector.Inject(level);
            var parent = level.transform;

            for (var i = 0; i < level.Areas.Count; i++)
            {
                var color = _options.AreasGradient.Evaluate(RandomExtensions.RandomOne());
                var maskData = _maskRenderOptions.Get(i);
                
                level.Areas[i].Setup(color, maskData, parent);
            }
            
            return level;
        }
    }
}