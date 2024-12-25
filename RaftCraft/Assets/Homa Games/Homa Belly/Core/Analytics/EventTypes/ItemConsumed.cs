namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class ItemConsumed : ItemEvent 
    {
        public ItemFlowReason Reason { get; }
        public string Reference { get; }
        
        public ItemConsumed(string itemId, int itemLevel, ItemFlowReason reason, string reference = "") : base(itemId, itemLevel)
        {
            this.Reason = reason;
            this.Reference = reference;
        }
    }
}