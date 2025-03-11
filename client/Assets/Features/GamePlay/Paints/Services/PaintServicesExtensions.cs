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
                .As<IPaintFactory>();

            var platformOptions = builder.GetOptions<PlatformOptions>();
            
            if (platformOptions.IsMobile == true)
            {
                builder.Register<MobilePaintDragStarter>()
                    .As<IPaintDragStarter>();
            }
            else
            {
                builder.Register<DesktopPaintDragStarter>()
                    .As<IPaintDragStarter>();
            }
            
            builder.Register<PaintCollection>()
                .As<IPaintCollection>();
            
            return builder;
        }
    }
}