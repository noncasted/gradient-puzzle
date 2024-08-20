using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Global.Publisher
{
    public class YandexCallbacks : MonoBehaviour
    {
        private bool _isInitialized;

        public event Action<string> UserDataReceived;
        public event Action<string> LeaderboardsReceived;
        public event Action InterstitialShown;
        public event Action<string> InterstitialFailed;
        public event Action RewardedAdClosed;
        public event Action<string> RewardedAdError;
        public event Action<IReadOnlyList<InternalProduct>> ProductsReceived;
        public event Action<IReadOnlyList<InternalPurchase>> PurchasesReceived;
        public event Action<string> PurchaseSuccess;
        public event Action<string> PurchaseFailed;
        public event Action Reviewed;

        public bool IsInitialized
        {
            get
            {
#if UNITY_EDITOR
                return true;
#endif

                return _isInitialized;
            }
        }

        public void OnInitialized()
        {
            _isInitialized = true;
        }

        public void OnUserDataReceived(string data)
        {
            UserDataReceived?.Invoke(data);
        }

        public void OnLeaderboardsReceived(string data)
        {
            LeaderboardsReceived?.Invoke(data);
        }

        public void OnInterstitialShown()
        {
            InterstitialShown?.Invoke();
        }

        public void OnInterstitialError(string error)
        {
            InterstitialFailed?.Invoke(error);
        }

        public void OnRewardedClose()
        {
            RewardedAdClosed?.Invoke();
        }

        public void OnRewardedError(string error)
        {
            RewardedAdError?.Invoke(error);
        }

        public void OnPurchaseSuccess(string id)
        {
            PurchaseSuccess?.Invoke(id);
        }

        public void OnPurchaseFailed(string error)
        {
            PurchaseFailed?.Invoke(error);
        }

        public void OnProductsReceived(string rawProduct)
        {
            var products = JsonConvert.DeserializeObject<InternalProduct[]>(rawProduct);

            ProductsReceived?.Invoke(products);
        }

        public void OnPurchasesReceived(string rawPurchases)
        {
            var purchases = JsonConvert.DeserializeObject<InternalPurchase[]>(rawPurchases);

            PurchasesReceived?.Invoke(purchases);
        }

        public void OnReview()
        {
            Reviewed?.Invoke();
        }
    }
}