namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class ResourceFlow : ResourceEvent 
    {
        public ResourceFlowType FlowType { get; }
        public ResourceFlowReason FlowReason { get; }
        public float FlowAmount { get; }
        public string Reference  { get; }
        
        public ResourceFlow(ResourceFlowType flowType, string currency, float flowAmount, float finalAmount, ResourceFlowReason flowReason, string reference = "")
        : base(currency, finalAmount)
        {
            this.FlowType = flowType;
            this.FlowAmount = flowAmount;
            this.FlowReason = flowReason;
            this.Reference = reference;
        }
    }
}