using Internal;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Common.UniversalAnimators
{
    [InlineEditor]
    [CreateAssetMenu(fileName = "AnimationData", menuName = "Common/Animator/Animation")]
    public class BaseAnimationData : EnvAsset, IAnimationData
    {
        [SerializeField] private AnimationClip _clip;
        [SerializeField] private float _time;
        [SerializeField] private float _fadeDuration;
        [SerializeField] private LayerDefinition _layer;

        public int AssetId => Id;
        public AnimationClip Clip => _clip;
        public float Time => _time;
        public float FadeDuration => _fadeDuration;
        public int Layer => _layer.Value;

        public IAnimation CreateAnimation()
        {
            var animation = new BaseAnimation(this);
            return animation;
        }
    }
}