using Features.Services.Inputs;
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

            builder.Register<GameInput>()
                .As<IGameInput>()
                .As<IScopeSetup>();
            
            return builder;
        }
    }
}