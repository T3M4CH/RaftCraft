using System;

namespace Game.Scripts.NPC.Interface
{
    public interface IHeals
    {
        public event Action<float> OnUpdateHealsProgress;
    }
}
