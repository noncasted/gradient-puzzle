using Features.Services.Inputs;
using Features.Services.RenderOptions;
using Internal;

namespace Features.Services
{
    public static class GamePlayServicesExtensions
    {
        public static IScopeBuilder AddGamePlayServices(this IScopeBuilder builder)
        {
            builder.Register<LevelsStorage>()
                .WithAsset<LevelsStorageOptions>()
                .As<ILevelsStorage>()
                .As<IScopeSetupAsync>();

            builder.Register<GameInput>()
                .As<IGameInput>()
                .As<IScopeSetup>();

            builder.RegisterAsset<MaskRenderOptions>()
                .As<IMaskRenderOptions>();
            
            return builder;
        }
    }
}