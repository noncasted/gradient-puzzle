using Features.Common.StateMachines;
using Features.Common.StateMachines.Abstract;
using Internal;

namespace Features.GamePlay
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