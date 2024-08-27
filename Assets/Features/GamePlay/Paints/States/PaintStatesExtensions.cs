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

            builder.Register<PaintDrag>()
                .WithAsset<PaintDragDefinition>()
                .WithAsset<PaintDragOptions>()
                .As<IPaintDrag>();

            builder.Register<PaintDrop>()
                .WithAsset<PaintDropDefinition>()
                .WithAsset<PaintDropOptions>()
                .As<IPaintDrop>();

            builder.Register<PaintReturn>()
                .WithAsset<PaintReturnDefinition>()
                .WithAsset<PaintReturnOptions>()
                .As<IPaintReturn>();

            builder.Register<PaintAnchoring>()
                .WithAsset<PaintAnchoringDefinition>()
                .WithAsset<PaintAnchoringOptions>()
                .As<IPaintAnchoring>();

            builder.Register<PaintComplete>()
                .WithAsset<PaintCompleteDefinition>()
                .WithAsset<PaintCompleteOptions>()
                .As<IPaintComplete>();

            return builder;
        }
    }
}