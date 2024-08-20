using System.Runtime.InteropServices;
using Internal;

namespace Global.Publisher
{
    public class YandexInitialization : IScopeBaseSetup
    {
        [DllImport("__Internal")]
        private static extern string InitializeYandexSDK();
        
        public void OnBaseSetup(IReadOnlyLifetime lifetime)
        {
            InitializeYandexSDK();
        }
    }
}