using UnityEngine;

namespace Global.Publisher.Yandex
{
    [DisallowMultipleComponent]
    public class YandexDebugCanvas : MonoBehaviour
    {
        [SerializeField] private AdsDebug _ads;
        [SerializeField] private PurchaseDebug _purchases;
        [SerializeField] private ReviewsDebug _reviews;

        public AdsDebug Ads => _ads;
        public PurchaseDebug Purchase => _purchases;
        public ReviewsDebug Reviews => _reviews;
    }
}