using Cysharp.Threading.Tasks;

namespace Global.Publisher
{
    public class ItchAds : IAds
    {
        public UniTask ShowInterstitial()
        {
            return UniTask.CompletedTask;
        }

        public UniTask<RewardAdResult> ShowRewarded()
        {
            return default;
        }
    }
}