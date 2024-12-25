namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class IAPPurchaseCompleted : IAPProductEvent
    {
        public string TransactionId { get; }
        public IAPPurchaseCompleted(ProductCategory productCategory, string productId, CurrencyType currencyType, int amount, string transactionId) : base(productCategory, productId, currencyType, amount)
        {
            TransactionId = transactionId;
        }
    }
}