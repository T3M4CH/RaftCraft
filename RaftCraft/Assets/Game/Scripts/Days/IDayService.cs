using System;

namespace Game.Scripts.Days
{
    public interface IDayService
    {
        public event Action<int> OnDayStart;
        public event Action<int> OnDayComplete;
        public void CompleteDay();
        public void RestartDay();
        void SetDay(int id);
        
        public int AttemptsCount { get; }
        public int CurrentDay { get; }
    }
}
