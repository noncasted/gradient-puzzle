using System.Runtime.InteropServices;

namespace Global.Publisher.Yandex
{
    public class PurchasesExternAPI : IPurchasesAPI
    {
        [DllImport("__Internal")]
        private static extern void Purchase(string id);

        [DllImport("__Internal")]
        private static extern void GetProducts();
        
        public void TryPurchase_Internal(string id)
        {
            Purchase(id);
        }

        public void GetProducts_Internal()
        {
            GetProducts();
        }
    }
}