namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class InterstitialAdClicked : InterstitialAdEvent
    {
        public InterstitialAdClicked(string adName, string impressionId, int levelId, AdPlacementType adPlacementType) :
            base(adName, impressionId, levelId, adPlacementType)
        {
        }
    }
}