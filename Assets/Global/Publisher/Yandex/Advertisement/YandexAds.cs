using Cysharp.Threading.Tasks;
using Global.Audio.Abstract;
using Global.Saves;
using Global.Systems;
using Internal;

namespace Global.Publisher
{
    public class YandexAds : IAds, IDataStorageLoadListener
    {
        private YandexAds(
            YandexCallbacks callbacks,
            IAdsAPI api,
            IProductLink adsProduct,
            IGlobalVolume volume, 
            IUpdater updater)
        {
            _callbacks = callbacks;
            _api = api;
            _adsProduct = adsProduct;
            _volume = volume;
            _updater = updater;
        }

        private readonly IAdsAPI _api;
        private readonly IProductLink _adsProduct;
        private readonly IGlobalVolume _volume;
        private readonly IUpdater _updater;
        private readonly YandexCallbacks _callbacks;

        private AdsSave _save;
        private IDataStorage _dataStorage;

        public async UniTask OnDataStorageLoaded(IReadOnlyLifetime lifetime, IDataStorage dataStorage)
        {
            _dataStorage = dataStorage;
            _save = await _dataStorage.GetEntry<AdsSave>();
            
            Msg.Listen<PurchaseEvent>(lifetime, OnProductUnlocked);
        }

        public async UniTask ShowInterstitial()
        {
            if (_save.IsDisabled == true)
                return;
            
            _updater.Pause();   
            _volume.Mute();
           
            var handler = new InterstitialHandler(_callbacks, _api);
            await handler.Show();
            
            _volume.Unmute();
            _updater.Continue();    
        }

        public async UniTask<RewardAdResult> ShowRewarded()
        {
            _updater.Pause();   
            var handler = new RewardedHandler(_callbacks, _api);
            var result = await handler.Show();
            _updater.Continue();   

            return result;
        }

        private void OnProductUnlocked(PurchaseEvent purchase)
        {
            if (purchase.ProductLink != _adsProduct)
                return;

            _save.IsDisabled = false;
            _dataStorage.Save(_save).Forget();
        }
    }
}