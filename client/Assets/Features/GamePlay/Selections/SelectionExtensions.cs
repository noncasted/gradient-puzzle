using Internal;

namespace GamePlay.Selections
{
    public static class SelectionExtensions
    {
        public static IScopeBuilder AddSelection(this IScopeBuilder builder)
        {
            builder.Register<PaintSelection>()
                .WithAsset<PaintSelectionOptions>()
                .As<IPaintSelection>();
            
            return builder;
        }
    }
}