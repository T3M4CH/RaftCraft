namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class MissionCompleted : MissionEvent
    {
        public string Reward { get; }
        public MissionCompleted(string missionId, string reward, int levelId) : base(missionId, levelId)
        {
            Reward = reward;
        }
    }
}