namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class SessionPlayed : SessionEvent
    {
        public int SessionNumber { get; }
        public long SessionLength { get; }
        public SessionPlayed(int sessionNumber, long sessionLength)
        {
            SessionNumber = sessionNumber;
            SessionLength = sessionLength;
        }
    }
}