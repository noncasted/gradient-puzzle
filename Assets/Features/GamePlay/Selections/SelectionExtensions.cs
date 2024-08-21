using Internal;

namespace Features.GamePlay
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