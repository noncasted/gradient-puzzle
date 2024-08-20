using Internal;
using UnityEngine;

namespace Global.Publisher
{
    public class GlobalPublisherOptions : EnvAsset
    {
        [SerializeField] private ItchCallbacks _itchCallbacksPrefab;
        [SerializeField] private YandexCallbacks _yandexCallbacksPrefab;
        [SerializeField] [CreateSO] private ShopProductsRegistry _yandexProductsRegistry;
        [SerializeField] [CreateSO] private ProductLink _yandexAdsDisableProduct;

        public ItchCallbacks ItchCallbacksPrefab => _itchCallbacksPrefab;
        public YandexCallbacks YandexCallbacksPrefab => _yandexCallbacksPrefab;
        public ShopProductsRegistry YandexProductsRegistry => _yandexProductsRegistry;
        public ProductLink YandexAdsDisableProduct => _yandexAdsDisableProduct;
    }
}