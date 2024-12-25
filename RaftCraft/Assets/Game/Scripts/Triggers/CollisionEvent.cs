using System;
using Game.Scripts.Extension;
using UnityEngine;

namespace Game.Scripts.Triggers
{
    public class CollisionEvent : MonoBehaviour
    {
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private SphereCollider _collider;
        public event Action<Collider> OnTriggerEnterEvent;
        public event Action<Collider> OnTriggerStayEvent; 
        public event Action<Collider> OnTriggerExitEvent;

        public void SetRadius(float radius)
        {
            _collider.radius = Mathf.Clamp(radius, 0f, float.MaxValue);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (_layerMask.Includes(other.gameObject.layer))
            {
                OnTriggerEnterEvent?.Invoke(other);
            }
        }


        private void OnTriggerStay(Collider other)
        {
            if (_layerMask.Includes(other.gameObject.layer))
            {
                OnTriggerStayEvent?.Invoke(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_layerMask.Includes(other.gameObject.layer))
            {
                OnTriggerExitEvent?.Invoke(other);
            }
        }
    }
}
