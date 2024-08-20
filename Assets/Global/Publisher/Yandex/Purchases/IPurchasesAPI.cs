namespace Global.Publisher.Yandex
{
    public interface IPurchasesAPI
    {
        void TryPurchase_Internal(string id);
        void GetProducts_Internal();
    }
}