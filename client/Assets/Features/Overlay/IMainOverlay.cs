using Cysharp.Threading.Tasks;
using Global.UI;
using Internal;
using Services;

namespace Overlay
{
    public interface IMainOverlay : IUIState 
    {
        IViewableDelegate ResetClicked { get; }
        IViewableDelegate<ILevelData> LevelSelected { get; }

        UniTask ShowSections();
        void ShowReset();
        void HideReset();
    }
}