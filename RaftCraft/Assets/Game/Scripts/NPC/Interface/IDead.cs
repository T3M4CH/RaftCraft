using System;
using Game.Scripts.Player.EntityGame;

namespace Game.Scripts.NPC.Interface
{
    public interface IDead<T> where T : Entity
    {
        public event Action<T> OnDead;
    }
}
