using UnityEngine;

namespace Game.Scripts.NPC
{
    public interface ICheckDistance
    {
        public bool CheckDistance(Vector3 target, float reachedDistance);
    }
}