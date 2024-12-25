namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class SessionStarted : SessionEvent
    {
        public int SessionNumber { get; }
        public float OfflineTime { get; }
        public SessionStarted(int sessionNumber, float offlineTime)
        {
            SessionNumber = sessionNumber;
            OfflineTime = offlineTime;
        }
    }
}