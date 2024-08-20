using Animancer;
using Cysharp.Threading.Tasks;
using Internal;

namespace GamePlay.Common
{
    public class EnhancedAnimator : IEnhancedAnimator
    {
        public EnhancedAnimator(AnimancerComponent animator)
        {
            _animator = animator;
        }

        private readonly AnimancerComponent _animator;

        public void PlayLooped(IReadOnlyLifetime lifetime, IAnimation animation)
        {
            var data = animation.Data;
            var state = _animator.Play(data.Clip, data.FadeDuration);
            state.Duration = data.Time;
   
            lifetime.Listen(state.Stop);
        }

        public async UniTask PlayAsync(IReadOnlyLifetime lifetime, IAnimation animation)
        {
            var data = animation.Data;
            var state = _animator.Play(data.Clip, data.FadeDuration, FadeMode.FromStart);
            state.Duration = data.Time;
            var completion = new UniTaskCompletionSource();
            
            state.Events.OnEnd += OnEnd;

            lifetime.Listen(OnCancelled);

            await completion.Task;
            
            state.Events.OnEnd -= OnEnd;

            return;

            void OnEnd()
            {
                completion.TrySetResult();
            }

            void OnCancelled()
            {
                completion.TrySetCanceled();
                state.Events.OnEnd -= OnEnd;
            }
        }

        public async UniTask PlayAsync(IReadOnlyLifetime lifetime, IAnimation animation, float time)
        {
            var data = animation.Data;
            var state = _animator.Play(data.Clip, data.FadeDuration, FadeMode.FromStart);
            state.Duration = time;
            var completion = new UniTaskCompletionSource();
            

            state.Events.OnEnd += OnEnd;

            lifetime.Listen(OnCancelled);

            await completion.Task;
            
            state.Events.OnEnd -= OnEnd;

            return;

            void OnEnd()
            {
                completion.TrySetResult();
            }

            void OnCancelled()
            {
                completion.TrySetCanceled();
                state.Events.OnEnd -= OnEnd;
            }
        }
    }
}