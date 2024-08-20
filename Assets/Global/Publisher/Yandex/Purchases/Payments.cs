using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Global.Publisher.Yandex
{
    public class Payments : IPayments
    {
        public Payments(IPurchasesAPI api, ShopProductsRegistry productsRegistry, YandexCallbacks callbacks)
        {
            _api = api;
            _productsRegistry = productsRegistry;
            _callbacks = callbacks;
        }

        private readonly IPurchasesAPI _api;
        private readonly ShopProductsRegistry _productsRegistry;
        private readonly YandexCallbacks _callbacks;

        public async UniTask<IReadOnlyList<IProductLink>> RestorePurchases()
        {
            var completion = new UniTaskCompletionSource<IReadOnlyList<InternalPurchase>>();

            _callbacks.PurchasesReceived += OnPurchasesReceived;

            _api.GetProducts_Internal();

            void OnPurchasesReceived(IReadOnlyList<InternalPurchase> products)
            {
                completion.TrySetResult(products);
            }

            var internalProducts = await completion.Task;
            
            _callbacks.PurchasesReceived -= OnPurchasesReceived;

            var productsToIds = internalProducts.ToDictionary(product => product.productID);
            var purchases = new List<IProductLink>();

            foreach (var productLink in _productsRegistry.Objects)
            {
                if (productsToIds.ContainsKey(productLink.Id) == false)
                    continue;
                
                purchases.Add(productLink);
            }

            return purchases;
        }

        public async UniTask ValidateProducts()
        {
            var completion = new UniTaskCompletionSource<IReadOnlyList<InternalProduct>>();

            _callbacks.ProductsReceived += OnProductsReceived;

            _api.GetProducts_Internal();

            void OnProductsReceived(IReadOnlyList<InternalProduct> products)
            {
                completion.TrySetResult(products);
            }

            var internalProducts = await completion.Task;
            var productsToIds = internalProducts.ToDictionary(product => product.id);

            foreach (var productLink in _productsRegistry.Objects)
            {
                if (productsToIds.TryGetValue(productLink.Id, out var internalProduct) == false)
                    continue;

                if (int.TryParse(internalProduct.priceValue, out var parseResult) == false)
                {
                    Debug.LogError($"Yandex shop: failed to parse price: {internalProduct.priceValue}");
                    continue;
                }

                productLink.UpdatePrice(parseResult);
            }

            _callbacks.ProductsReceived -= OnProductsReceived;
        }

        public async UniTask<PurchaseResult> TryPurchase(IProductLink productLink)
        {
            var targetId = productLink.Id;
            var completion = new UniTaskCompletionSource<PurchaseResult>();

            _callbacks.PurchaseSuccess += OnSuccess;
            _callbacks.PurchaseFailed += OnFail;

            _api.TryPurchase_Internal(targetId);

            void OnSuccess(string id)
            {
                Debug.Log($"Received success payment id: {targetId}");

                if (targetId != id)
                    return;

                completion.TrySetResult(PurchaseResult.Success);
            }

            void OnFail(string message)
            {
                Debug.LogError(message);

                completion.TrySetResult(PurchaseResult.Fail);
            }

            var result = await completion.Task;

            _callbacks.PurchaseSuccess -= OnSuccess;
            _callbacks.PurchaseFailed -= OnFail;

            return result;
        }
    }
}