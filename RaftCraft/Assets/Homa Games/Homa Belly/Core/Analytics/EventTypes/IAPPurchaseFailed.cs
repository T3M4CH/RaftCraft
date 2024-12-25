namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class IAPPurchaseFailed : IAPProductEvent
    {
        public IAPPurchaseFailed(ProductCategory productCategory, string productId, CurrencyType currencyType, int amount) : base(productCategory, productId, currencyType, amount)
        {
        }
    }
}