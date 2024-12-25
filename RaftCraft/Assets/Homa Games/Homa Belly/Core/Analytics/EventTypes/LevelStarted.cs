using System.Collections.Generic;

namespace HomaGames.HomaBelly.Internal.Analytics
{
    public class LevelStarted : ProgressionEvent
    {
        public LevelStarted(int levelId) : base(levelId)
        {
        }
    }
}