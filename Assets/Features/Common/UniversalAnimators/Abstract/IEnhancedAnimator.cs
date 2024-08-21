using Cysharp.Threading.Tasks;
using Internal;

namespace Features.Common.UniversalAnimators.Abstract
{
    public interface IEnhancedAnimator
    {
        void PlayLooped(IReadOnlyLifetime lifetime, IAnimation animation);
        UniTask PlayAsync(IReadOnlyLifetime lifetime, IAnimation animation);
        UniTask PlayAsync(IReadOnlyLifetime lifetime, IAnimation animation, float time);
    }
}