using Features.GamePlay.Mover;
using Internal;

namespace Features.GamePlay
{
    public static class PaintServicesExtensions
    {
        public static IScopeBuilder AddPaintServices(this IScopeBuilder builder)
        {
            builder.Register<PaintFactory>()
                .WithAsset<PaintFactoryOptions>()
                .WithParameter(builder.Scope)
                .As<IPaintFactory>();

            builder.Register<PaintMover>()
                .As<IPaintMover>();

            return builder;
        }
    }
}