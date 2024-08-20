using Sirenix.OdinInspector;
using UnityEngine;

namespace GamePlay.Common
{
    [InlineEditor]
    [CreateAssetMenu(fileName = "AnimatorLayer", menuName = "Common/Animator/AnimatorLayer")]
    public class LayerDefinition : ScriptableObject, ILayerDefinition
    {
        [SerializeField] private int _value;

        public int Value => _value;
    }
}