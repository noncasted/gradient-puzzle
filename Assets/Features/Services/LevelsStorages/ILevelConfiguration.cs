using Features.GamePlay;
using Internal;

namespace Features.Services
{
    public interface ILevelConfiguration
    {
        IViewableProperty<bool> IsUnlocked { get; }
        
        int Index { get; }
        Level Prefab { get; }
    }
}