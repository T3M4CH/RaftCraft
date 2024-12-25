namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class InterstitialAdReady : InterstitialAdEvent
    {
        public InterstitialAdReady(string adName, string impressionId, int levelId, AdPlacementType adPlacementType) 
            : base(adName, impressionId, levelId, adPlacementType)
        {
        }
    }
}