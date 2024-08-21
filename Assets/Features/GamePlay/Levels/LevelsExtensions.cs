using Internal;

namespace Features.GamePlay
{
    public static class LevelsExtensions
    {
        public static IScopeBuilder AddLevels(this IScopeBuilder builder)
        {
            builder.Register<LevelLoader>()
                .WithAsset<LevelLoaderOptions>()
                .As<ILevelLoader>();
            
            return builder;
        }
    }
}