using Internal;

namespace Features.Services
{
    public static class GamePlayServicesExtensions
    {
        public static IScopeBuilder AddGamePlayServices(this IScopeBuilder builder)
        {
            builder.Register<LevelsStorage>()
                .WithAsset<LevelsStorageOptions>()
                .As<ILevelsStorage>();

            return builder;
        }
    }
}