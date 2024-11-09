using Global.UI;

namespace Overlay
{
    public interface IOverlayBackground
    {
        void Show(IUIState source);
        void Hide(IUIState source);
    }

    public static class OverlayBackgroundExtensions
    {
        public static void Show(this IOverlayBackground background, IUIStateHandle source)
        {
            background.Show(source.State);
            source.InnerLifetime.Listen(() => background.Hide(source.State));
        }
    }
}