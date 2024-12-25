namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class RewardedVideoAdSuggested : RewardedAdEvent
    {
        public RewardedVideoAdSuggested(string adName, string impressionId, int levelId, AdPlacementType adPlacementType) : base(adName, impressionId, levelId, adPlacementType)
        {
        }
    }
}