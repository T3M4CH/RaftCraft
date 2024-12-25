namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class Checkpoint : ProgressionEvent
    {
        public string CheckpointId { get; }
        public Checkpoint(string checkpointId, int levelId) : base(levelId)
        {
            CheckpointId = checkpointId;
        }
    }
}