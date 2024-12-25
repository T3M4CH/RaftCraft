namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class IAPPurchaseStarted : IAPProductEvent
    {
        public IAPPurchaseStarted(ProductCategory productCategory, string productId, CurrencyType currencyType, int amount) : base(productCategory, productId, currencyType, amount)
        {
            
        }
    }
}