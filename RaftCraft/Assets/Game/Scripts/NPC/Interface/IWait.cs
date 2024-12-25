using System;

namespace Game.Scripts.NPC
{
    public interface IWait
    {
        public void Wait(Action callback, float time);

        public void Trigger();
    }
}