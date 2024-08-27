using Cysharp.Threading.Tasks;
using Features.Services;
using Global.UI;
using Internal;

namespace Features
{
    public interface IMainOverlay : IUIState 
    {
        IViewableDelegate ResetClicked { get; }
        IViewableDelegate<ILevelConfiguration> LevelSelected { get; }

        void ShowReset();
        void HideReset();
    }
}