namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class MainMenuLoaded : SessionEvent
    {
        public long TotalGameplaySeconds { get; }
        public MainMenuLoaded(long gameplaySeconds)
        {
            TotalGameplaySeconds = gameplaySeconds;
        }
    }
}