namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class InterstitialAdShowSucceeded : InterstitialAdEvent
    {
        public InterstitialAdShowSucceeded(string adName, string impressionId, int levelId,
            AdPlacementType adPlacementType) : base(adName, impressionId, levelId, adPlacementType)
        {
        }
    }
}