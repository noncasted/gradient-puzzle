using Global.UI;
using Internal;
using Services;

namespace Overlay
{
    public interface IMainOverlay : IUIState 
    {
        IViewableDelegate ResetClicked { get; }
        IViewableDelegate<ILevelConfiguration> LevelSelected { get; }

        void ShowReset();
        void HideReset();
    }
}