using Cysharp.Threading.Tasks;
using Global.UI;
using Services;

namespace Menu.Levels
{
    public interface ILevelSelection : IUIState
    {
        UniTask<ILevelData> Show(IUIStateHandle handle, LevelSectionType sectionType);
    }
}