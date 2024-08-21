using Internal;

namespace Features.GamePlay
{
    public static class PaintStatesExtensions
    {
        public static IEntityBuilder AddStates(this IEntityBuilder builder)
        {
            builder.Register<PaintSpawn>()
                .WithAsset<PaintSpawnDefinition>()
                .WithAsset<PaintSpawnOptions>()
                .As<IPaintSpawn>();
            
            return builder;
        }
    }
}