using Internal;

namespace Services
{
    public static class GamePlayServicesExtensions
    {
        public static IScopeBuilder AddGamePlayServices(this IScopeBuilder builder)
        {
            builder.Register<LevelsStorage>()
                .WithAsset<LevelsStorageOptions>()
                .As<ILevelsStorage>()
                .As<IScopeSetupAsync>();

            var platformOptions = builder.GetOptions<PlatformOptions>();
         
            if (platformOptions.IsMobile == true)
            {
                builder.Register<MobileGameInput>()
                    .As<IGameInput>()
                    .As<IScopeSetup>();
            }
            else
            {
                builder.Register<DesktopGameInput>()
                    .As<IGameInput>()
                    .As<IScopeSetup>();
            }

            builder.RegisterAsset<MaskRenderOptions>()
                .As<IMaskRenderOptions>();
            
            return builder;
        }
    }
}