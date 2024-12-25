namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class LevelFirstCompletion : ProgressionEvent
    {
        public long LevelDuration { get; }
        public int Attempts { get; }
        public LevelFirstCompletion(int levelId, long levelDuration, int attempts) : base(levelId)
        {
            LevelDuration = levelDuration;
            Attempts = attempts;
        }
    }
}