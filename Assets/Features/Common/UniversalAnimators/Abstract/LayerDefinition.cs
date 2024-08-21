using Sirenix.OdinInspector;
using UnityEngine;

namespace Features.Common.UniversalAnimators.Abstract
{
    [InlineEditor]
    [CreateAssetMenu(fileName = "AnimatorLayer", menuName = "Common/Animator/AnimatorLayer")]
    public class LayerDefinition : ScriptableObject, ILayerDefinition
    {
        [SerializeField] private int _value;

        public int Value => _value;
    }
}