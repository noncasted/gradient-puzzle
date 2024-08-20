using Cysharp.Threading.Tasks;
using Internal;

namespace GamePlay.Common
{
    public interface IEnhancedAnimator
    {
        void PlayLooped(IReadOnlyLifetime lifetime, IAnimation animation);
        UniTask PlayAsync(IReadOnlyLifetime lifetime, IAnimation animation);
        UniTask PlayAsync(IReadOnlyLifetime lifetime, IAnimation animation, float time);
    }
}