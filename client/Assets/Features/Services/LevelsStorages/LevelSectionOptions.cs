using Internal;
using Shared;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Services
{
    [InlineEditor]
    public class LevelSectionOptions : EnvAsset
    {
        [SerializeField] private LevelSectionType _type;
        [SerializeField] private Sprite _preview;
        [SerializeField] private Gradient _previewGradient;
        
        public LevelSectionType Type => _type;
        public Sprite Preview => _preview;
        public Gradient PreviewGradient => _previewGradient;
    }
}