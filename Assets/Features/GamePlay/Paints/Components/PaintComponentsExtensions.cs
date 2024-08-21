using Features.Common.StateMachines;
using Features.Common.StateMachines.Abstract;
using Internal;

namespace Features.GamePlay.Paints
{
    public static class PaintComponentsExtensions
    {
        public static IEntityBuilder AddComponents(this IEntityBuilder builder)
        {
            builder.Register<StateMachine>()
                .As<IStateMachine>();
            
            return builder;
        }
    }
}