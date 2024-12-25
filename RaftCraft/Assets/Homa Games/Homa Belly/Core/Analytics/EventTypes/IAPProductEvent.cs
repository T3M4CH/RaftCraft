namespace HomaGames.HomaBelly.Internal.Analytics
{
    public abstract class IAPProductEvent : IAPEvent
    {
        public ProductCategory ProductCategory { get; }
        public string ProductId { get; }
        public CurrencyType CurrencyType { get; }
        public int Amount { get; }
        protected IAPProductEvent(ProductCategory productCategory, string productId, CurrencyType currencyType, int amount)
        {
            ProductCategory = productCategory;
            ProductId = productId;
            CurrencyType = currencyType;
            Amount = amount;
        }
    }
}