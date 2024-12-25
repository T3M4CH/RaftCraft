using System;

namespace Game.Scripts.Quest.Interfaces
{
    public interface IReadyClient
    {
        public event Action<IReadyClient> OnChangeReady;
        public bool IsReady { get; }
        public float Delay { get; }
    }
}