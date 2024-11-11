using Internal;

namespace GamePlay.Paints
{
    public static class PaintServicesExtensions
    {
        public static IScopeBuilder AddPaintServices(this IScopeBuilder builder)
        {
            builder.Register<PaintFactory>()
                .WithAsset<PaintFactoryOptions>()
                .WithParameter(builder.Container)
                .As<IPaintFactory>();

            builder.Register<PaintDragStarter>()
                .As<IPaintDragStarter>();

            return builder;
        }
    }
}