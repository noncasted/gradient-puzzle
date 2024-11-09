using GamePlay.Levels;
using Internal;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Services
{
    [InlineEditor]
    public class LevelConfiguration : EnvAsset, ILevelConfiguration
    {
        [SerializeField] private Level _prefab;

        private readonly ViewableProperty<bool> _isUnlocked = new();

        private int _index;

        public IViewableProperty<bool> IsUnlocked => _isUnlocked;
        public int Index => _index;
        public Level Prefab => _prefab;
        
        public void Setup(int index)
        {
            _index = index;
        }
        
        public void OnUnlocked()
        {
            _isUnlocked.Set(true);
        }
    }
}