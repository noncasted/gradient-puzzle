using GamePlay.Levels;
using Internal;

namespace Services
{
    public class LevelData : ILevelData
    {
        public LevelData(LevelOptions options, int index)
        {
            Id = options.Id.ToString();
            Index = index;
            SectionType = options.SectionType;
            Prefab = options.Prefab;
        }

        private readonly LevelOptions _options;
        private readonly ViewableProperty<bool> _isPassed = new();
        private readonly ViewableProperty<bool> _isUnlocked = new();

        public IViewableProperty<bool> IsPassed => _isPassed;
        public IViewableProperty<bool> IsUnlocked => _isUnlocked;

        public string Id { get; }
        public int Index { get; }

        public LevelSectionType SectionType { get; }
        public Level Prefab { get; }

        public void OnPassed()
        {
            _isPassed.Set(true);
            _isUnlocked.Set(true);
        }

        public void Unlock()
        {
            _isUnlocked.Set(false);
        }
    }
}