using System.Collections.Generic;

namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class IAPCurrentlyActive : IAPEvent
    {
        public List<string> ActiveProductIds { get; }
        public IAPCurrentlyActive(List<string> productIds)
        {
            ActiveProductIds = productIds;
        }
    }
}