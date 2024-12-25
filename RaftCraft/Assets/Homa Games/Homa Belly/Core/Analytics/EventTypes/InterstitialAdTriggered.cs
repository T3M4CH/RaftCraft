namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class InterstitialAdTriggered : InterstitialAdEvent
    {
        public InterstitialAdTriggered(string adName, string impressionId, int levelId, AdPlacementType adPlacementType) : base(adName, impressionId, levelId, adPlacementType)
        {
        }
    }
}