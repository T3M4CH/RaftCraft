namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class GameplayStarted: SessionEvent
    {
        public long TotalGameplaySeconds { get; }
        public GameplayStarted(long gameplaySeconds)
        {
            TotalGameplaySeconds = gameplaySeconds;
        }
    }
}