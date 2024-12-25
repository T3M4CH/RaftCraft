namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class InterstitialAdShowFailed : InterstitialAdEvent
    {
        public InterstitialAdShowFailed(string adName, string impressionId, int levelId,
            AdPlacementType adPlacementType) : base(adName, impressionId, levelId, adPlacementType)
        {
        }
    }
}