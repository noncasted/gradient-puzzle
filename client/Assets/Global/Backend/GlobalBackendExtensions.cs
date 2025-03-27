using Internal;

namespace Global.Backend
{
    public static class GlobalBackendExtensions
    {
        public static IScopeBuilder AddBackend(this IScopeBuilder builder)
        {
            builder.Register<BackendGetGateway>()
                .As<IBackendGetGateway>();

            builder.Register<BackendMediaGateway>()
                .As<IBackendMediaGateway>();
            
            builder.Register<BackendPostGateway>()
                .As<IBackendPostGateway>();
            
            builder.Register<BackendClient>()
                .As<IBackendClient>();

            builder.Register<AuthBackend>()
                .WithAsset<BackendOptions>()
                .As<IAuthBackend>();

            builder.Register<UserBackend>()
                .WithAsset<BackendOptions>()
                .As<IUserBackend>();
            
            return builder;
        }
    }
}