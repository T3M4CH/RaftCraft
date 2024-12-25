namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class MissionFailed : MissionEvent
    {
        public MissionFailed(string missionId, int levelId) : base(missionId, levelId)
        {
        }
    }
}