using GamePlay.Paints.Collection;
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

            builder.Register<PaintCollection>()
                .As<IPaintCollection>();
            
            return builder;
        }
    }
}