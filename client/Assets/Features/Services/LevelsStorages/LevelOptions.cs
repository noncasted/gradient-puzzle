using GamePlay.Levels;
using Internal;
using Shared;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Services
{
    [InlineEditor]
    public class LevelOptions : EnvAsset
    {
        [SerializeField] private Level _prefab;
        [SerializeField] private LevelSectionType _sectionType;
        [SerializeField] private Sprite _preview;
        
        public Level Prefab => _prefab;
        public LevelSectionType SectionType => _sectionType;
        public Sprite Preview => _preview;

        public void Setup(Level prefab, LevelSectionType section, Sprite preview)
        {
            _prefab = prefab;
            _sectionType = section;
            _preview = preview;
        }
    }
}