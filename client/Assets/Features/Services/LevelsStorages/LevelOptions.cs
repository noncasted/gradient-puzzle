using GamePlay.Levels;
using Internal;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Services
{
    [InlineEditor]
    public class LevelOptions : EnvAsset
    {
        [SerializeField] private Level _prefab;
        [SerializeField] private LevelSectionType _sectionType;
        [SerializeField] private DefaultAsset _image;
        
        public Level Prefab => _prefab;
        public LevelSectionType SectionType => _sectionType;
        public string ImagePath => AssetDatabase.GetAssetPath(_image);

        public void Setup(Level prefab, LevelSectionType section, DefaultAsset image)
        {
            _prefab = prefab;
            _sectionType = section;
            _image = image;
        }
    }
}