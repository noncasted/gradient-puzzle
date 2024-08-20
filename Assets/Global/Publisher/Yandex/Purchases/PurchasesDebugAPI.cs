using System.Collections.Generic;
using Newtonsoft.Json;

namespace Global.Publisher.Yandex
{
    public class PurchasesDebugAPI : IPurchasesAPI
    {
        public PurchasesDebugAPI(IPurchaseDebug debug, YandexCallbacks callbacks, ShopProductsRegistry productsRegistry)
        {
            _debug = debug;
            _callbacks = callbacks;
            _productsRegistry = productsRegistry;
        }

        private readonly IPurchaseDebug _debug;
        private readonly YandexCallbacks _callbacks;
        private readonly ShopProductsRegistry _productsRegistry;

        public void TryPurchase_Internal(string id)
        {
            _debug.Purchase(id);
        }

        public void GetProducts_Internal()
        {
            var products = new List<InternalProduct>();

            foreach (var product in _productsRegistry.Objects)
            {
                var internalProduct = new InternalProduct()
                {
                    id = product.Id,
                    price = $"{product.Price.ToString()} YAN",
                    priceValue = product.Price.ToString(),
                    priceCurrencyCode = "YAN"
                };
                
                products.Add(internalProduct);
            }
            
            _callbacks.OnProductsReceived(JsonConvert.SerializeObject(products.ToArray()));
        }
    }
}