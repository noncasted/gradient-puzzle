using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.GamePlay
{
    [InlineEditor]
    public class AreaRendererOptions : ScriptableObject
    {
        [SerializeField] private Color _outlineColor;
        [SerializeField] [Min(0f)] private float _outlineWidth;
        
         public Color OutlineColor => _outlineColor;
         public float OutlineWidth => _outlineWidth;
    }
}