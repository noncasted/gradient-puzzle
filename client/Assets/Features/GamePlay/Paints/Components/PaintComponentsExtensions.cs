using Common.StateMachines;
using Internal;

namespace GamePlay.Paints
{
    public static class PaintComponentsExtensions
    {
        public static IEntityBuilder AddComponents(this IEntityBuilder builder)
        {
            builder.Register<StateMachine>()
                .As<IStateMachine>()
                .As<IScopeSetup>();

            builder.Register<PaintInterceptor>()
                .As<IPaintInterceptor>()
                .AsSelf();

            builder.Register<PaintMover>()
                .WithAsset<PaintMoverOptions>()
                .As<IPaintMover>();
            
            return builder;
        }
    }
}