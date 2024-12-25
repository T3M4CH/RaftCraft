namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class RewardedVideoAdTriggered : RewardedAdEvent
    {
        public RewardedVideoAdTriggered(string adName, string impressionId, int levelId, AdPlacementType adPlacementType) : base(adName, impressionId, levelId, adPlacementType)
        {
        }
    }
}