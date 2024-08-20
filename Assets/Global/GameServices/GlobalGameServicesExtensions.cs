using Global.Publisher;
using Internal;

namespace Global.GameServices
{
    public static class GlobalGameServicesExtensions
    {
        public static IScopeBuilder AddGameServices(this IScopeBuilder builder)
        {
            var configsRegistry = builder.GetAsset<ConfigsRegistry>();

            foreach (var source in configsRegistry.Objects)
                source.CreateInstance(builder);

            builder.Register<Configs>()
                .As<IConfigs>();

            return builder;
        }
    }
}