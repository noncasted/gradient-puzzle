using Cysharp.Threading.Tasks;
using Internal;

namespace Common.UniversalAnimators
{
    public interface IEnhancedAnimator
    {
        void PlayLooped(IReadOnlyLifetime lifetime, IAnimation animation);
        UniTask PlayAsync(IReadOnlyLifetime lifetime, IAnimation animation);
        UniTask PlayAsync(IReadOnlyLifetime lifetime, IAnimation animation, float time);
    }
}