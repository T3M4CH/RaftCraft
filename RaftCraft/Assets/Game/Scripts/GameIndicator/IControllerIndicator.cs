using UnityEngine;

namespace Game.Scripts.GameIndicator
{
    public interface IControllerIndicator
    {
        public void AddTarget(Transform target, Vector3 position, float distanceThreshold, Sprite sprite, Color color, bool isStaticPosition = false, float multiplierOffset = 3f);
        public void RemoveTarget(Transform target);
    }
}
