using UnityEngine;

namespace Features.Common.UniversalAnimators.Abstract
{
    public interface IAnimationData
    {
        int AssetId { get; }
        AnimationClip Clip { get; }
        float Time { get; }
        float FadeDuration { get; }
        int Layer { get; }
    }
}