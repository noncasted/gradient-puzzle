using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Global.Publisher
{
    public class RewardedHandler
    {
        public RewardedHandler(
            YandexCallbacks callbacks,
            IAdsAPI adsAPI)
        {
            _callbacks = callbacks;
            _api = adsAPI;
        }

        private readonly YandexCallbacks _callbacks;
        private readonly IAdsAPI _api;

        public async UniTask<RewardAdResult> Show()
        {
            var completion = new UniTaskCompletionSource<RewardAdResult>();

            _callbacks.RewardedAdClosed += OnClosed;
            _callbacks.RewardedAdError += OnError;

            _api.ShowRewarded_Internal();

            var result = await completion.Task;

            _callbacks.RewardedAdClosed -= OnClosed;
            _callbacks.RewardedAdError -= OnError;

            return result;

            void OnError(string message)
            {
                Debug.LogError($"Interstitial failed: {message}");
                completion.TrySetResult(RewardAdResult.Error);
            }

            void OnClosed()
            {
                completion.TrySetResult(RewardAdResult.Applied);
            }
        }
    }
}