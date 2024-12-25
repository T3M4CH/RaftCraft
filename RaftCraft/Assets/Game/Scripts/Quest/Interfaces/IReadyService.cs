using System;

namespace Game.Scripts.Quest.Interfaces
{
    public interface IReadyService
    {
        public event Action<float> OnChangeTime; 
        public void AddListener(Action<ReadyState> action);
        public void RemoveListener(Action<ReadyState> action);
        public void AddClient(IReadyClient client);
        public void RemoveClient(IReadyClient client);
    }
}