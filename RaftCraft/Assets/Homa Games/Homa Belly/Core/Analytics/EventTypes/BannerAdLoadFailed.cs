namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class BannerAdLoadFailed : BannerAdEvent
    {
        public BannerAdLoadFailed(string impressionId, AdPlacementType adPlacementType) : base(impressionId, adPlacementType)
        {
        }
    }
}