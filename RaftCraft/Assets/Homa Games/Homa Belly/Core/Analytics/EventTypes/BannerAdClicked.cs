namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class BannerAdClicked : BannerAdEvent
    {
        public BannerAdClicked(string impressionId, AdPlacementType adPlacementType) : base(impressionId, adPlacementType)
        {
        }
    }
}