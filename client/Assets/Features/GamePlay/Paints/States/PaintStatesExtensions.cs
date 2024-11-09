using Internal;

namespace GamePlay.Paints
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
                .As<IPaintDrag>();

            builder.Register<PaintDrop>()
                .WithAsset<PaintDropDefinition>()
                .WithAsset<PaintDropOptions>()
                .As<IPaintDrop>();

            builder.Register<PaintReturn>()
                .WithAsset<PaintReturnDefinition>()
                .As<IPaintReturn>();

            builder.Register<PaintAnchoring>()
                .WithAsset<PaintAnchoringDefinition>()
                .As<IPaintAnchoring>();

            builder.Register<PaintComplete>()
                .WithAsset<PaintCompleteDefinition>()
                .WithAsset<PaintCompleteOptions>()
                .As<IPaintComplete>();

            return builder;
        }
    }
}