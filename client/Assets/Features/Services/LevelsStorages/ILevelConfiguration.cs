using GamePlay.Levels;
using Internal;

namespace Services
{
    public interface ILevelConfiguration
    {
        IViewableProperty<bool> IsUnlocked { get; }
        
        int Index { get; }
        Level Prefab { get; }
    }
}