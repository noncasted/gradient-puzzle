using GamePlay.Levels;
using Internal;
using Shared;

namespace Services
{
    public interface ILevelData
    {
        IViewableProperty<bool> IsPassed { get; }
        IViewableProperty<bool> IsUnlocked { get; }
        string Id { get; }
        LevelSectionType SectionType { get; }
        int Index { get; }
        Level Prefab { get; }
        LevelOptions Options { get; }

        void OnPassed();
        void Unlock();
    }
}