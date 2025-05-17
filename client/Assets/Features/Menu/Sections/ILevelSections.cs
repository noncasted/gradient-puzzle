using Cysharp.Threading.Tasks;
using Global.UI;
using Services;

namespace Menu.Sections
{
    public interface ILevelSections : IUIState
    {
        UniTask<ILevelData> Show(IUIStateHandle handle, bool withBackOptions);
    }
}