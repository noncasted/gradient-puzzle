using Cysharp.Threading.Tasks;
using Global.UI;
using Services;

namespace Overlay
{
    public interface ILevelSelectionUI : IUIState
    {
        UniTask<LevelSelectionResult> Process(IUIStateHandle handle);
    }

    public class LevelSelectionResult
    {
        public LevelSelectionResult()
        {
            IsSelected = false;
            Level = null;
        }
        
        public LevelSelectionResult(ILevelData level)
        {
            IsSelected = true;
            Level = level;
        }

        public bool IsSelected { get; }
        public ILevelData Level { get; }
    }
}