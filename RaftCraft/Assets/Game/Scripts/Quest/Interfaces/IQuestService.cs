using System;

namespace Game.Scripts.Quest.Interfaces
{
    public interface IQuestService
    {
        event Action<bool> OnStatusChanged;
        void SetButtonReady(bool value);
    }
}