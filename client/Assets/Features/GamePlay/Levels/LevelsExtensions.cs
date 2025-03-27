using Internal;

namespace GamePlay.Levels
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