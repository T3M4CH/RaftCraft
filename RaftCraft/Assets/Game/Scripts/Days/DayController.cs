using System;
using Game.GameBalanceCore.scripts;
using Game.Scripts.Saves;
using GTap.Analytics;


namespace Game.Scripts.Days
{
    public class DayController : IDisposable, IDayService, IStartableElement
    {
        public event Action<int> OnDayStart = _ => { };
        public event Action<int> OnDayComplete = _ => { };

        private readonly GameSave _saveController;

        public int AttemptsCount { get; private set; }

        private int _currentDay;

        public int CurrentDay
        {
            get => _currentDay;
            private set
            {
                _currentDay = value;
                GtapAnalytics.CurrentDay = _currentDay;
            }
        }

        public DayController(GameSave saveController)
        {
            _saveController = saveController;

            CurrentDay = saveController.GetData(SaveConstants.CurrentDay, 1);
            AttemptsCount = saveController.GetData(SaveConstants.AttemptsCount, 1);
            
            GameBalance.Instance.UpdateConfigByDay(CurrentDay);
        }
        
        public void CompleteDay()
        {
            GtapAnalytics.LevelComplete(CurrentDay);
            GtapAnalytics.CommitUnderwaterDeaths();

            OnDayComplete(CurrentDay);
            CurrentDay += 1;
            AttemptsCount = 1;
            OnDayStart.Invoke(CurrentDay);

            GameBalance.Instance.UpdateConfigByDay(CurrentDay);
            GtapAnalytics.LevelStart(CurrentDay);

            Save();
        }

        public void RestartDay()
        {
            GtapAnalytics.LevelFail(CurrentDay);

            AttemptsCount += 1;
            OnDayStart.Invoke(CurrentDay);

            GtapAnalytics.LevelStart(CurrentDay);

            Save();
        }
        
        public void SetDay(int id)
        {
            CurrentDay = id;
            OnDayStart.Invoke(CurrentDay);

            Save();
        }

        private void Save()
        {
            _saveController.SetData(SaveConstants.AttemptsCount, AttemptsCount);
            _saveController.SetData(SaveConstants.CurrentDay, CurrentDay);
        }

        public void Dispose()
        {
            OnDayStart = null;
            OnDayComplete = null;
        }

        public int Priority { get; } = 10000;

        public void Execute()
        {
            OnDayStart.Invoke(CurrentDay);
            GtapAnalytics.LevelStart(CurrentDay);
        }
    }
}