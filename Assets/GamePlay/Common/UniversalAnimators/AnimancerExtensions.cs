using System;
using Animancer;
using Cysharp.Threading.Tasks;
using Internal;

namespace GamePlay.Common
{
    public static class AnimancerExtensions
    {
        public static UniTask PlayAsync(
            this AnimancerComponent animator,
            IReadOnlyLifetime lifetime,
            BaseAnimationData animation) => animator.PlayAsync(lifetime, animation, animation.Time);

        public static UniTask PlayAsync(
            this AnimancerComponent animator,
            IReadOnlyLifetime lifetime,
            BaseAnimationData animation,
            float time) => animator.PlayAsync(lifetime, animation, time, null);

        public static UniTask PlayAsync(
            this AnimancerComponent animator,
            IReadOnlyLifetime lifetime,
            BaseAnimationData animation,
            Action<AnimancerState> stateCallback) =>
            animator.PlayAsync(lifetime, animation, animation.Time, stateCallback);

        public static async UniTask PlayAsync(
            this AnimancerComponent animator,
            IReadOnlyLifetime lifetime,
            BaseAnimationData animation,
            float time,
            Action<AnimancerState> stateCallback)
        {
            var state = animator.Play(animation.Clip, animation.FadeDuration, FadeMode.FromStart);
            state.Duration = time;
            stateCallback?.Invoke(state);

            var completion = new UniTaskCompletionSource();

            state.Events.OnEnd += OnEnd;
            Action callback = OnCancelled;
            lifetime.Listen(callback);

            await completion.Task;

            state.Events.OnEnd -= OnEnd;
            lifetime.RemoveListener(callback);

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