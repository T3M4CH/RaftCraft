namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class RewardedAdFirstWatchedSession : RewardedAdEvent
    {
        public long GameplaySeconds { get; }

        public RewardedAdFirstWatchedSession(string adName, string impressionId, int levelId,
            AdPlacementType adPlacementType, long gameplaySeconds) 
            : base(adName, impressionId, levelId, adPlacementType)
        {
            GameplaySeconds = gameplaySeconds;
        }
    }
}