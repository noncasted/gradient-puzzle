using GamePlay.Levels;
using Internal;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Services
{
    [InlineEditor]
    public class LevelOptions : EnvAsset
    {
        [SerializeField] private Level _prefab;
        [SerializeField] private LevelSectionType _sectionType;
            
        public Level Prefab => _prefab;
        public LevelSectionType SectionType => _sectionType;
    }
}