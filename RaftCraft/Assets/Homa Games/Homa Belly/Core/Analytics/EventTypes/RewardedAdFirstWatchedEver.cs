namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class RewardedAdFirstWatchedEver : RewardedAdEvent
    {
        public long GameplaySeconds { get; }

        public RewardedAdFirstWatchedEver(string adName, string impressionId, int levelId,
            AdPlacementType adPlacementType, long gameplaySeconds) 
            : base(adName, impressionId, levelId, adPlacementType)
        {
            GameplaySeconds = gameplaySeconds;
        }
    }
}