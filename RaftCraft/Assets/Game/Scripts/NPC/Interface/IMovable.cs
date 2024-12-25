using UnityEngine;

namespace Game.Scripts.NPC
{
    public interface IMovable
    {
        public void Reset();
        public void Move(Vector3 target, bool isClimb);
    }
}