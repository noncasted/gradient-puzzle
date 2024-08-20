using System.Runtime.InteropServices;

namespace Global.Publisher
{
    public class AdsExternAPI : IAdsAPI
    {
        [DllImport("__Internal")]
        private static extern void ShowFullscreenAd();

        [DllImport("__Internal")]
        private static extern int ShowRewardedAd();

        public void ShowInterstitial_Internal()
        {
            ShowFullscreenAd();
        }

        public void ShowRewarded_Internal()
        {
            ShowRewardedAd();
        }
    }
}