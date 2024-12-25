using System;
using UnityEngine;

namespace Game.Scripts.Player.Collision
{
    public class EntityCollision : MonoBehaviour
    {
        public event Action<Collider> OnEventTriggerEnter;
        public event Action<Collider> OnEventTriggerExit; 
        public event Action<UnityEngine.Collision> OnCollision; 

        private void OnTriggerEnter(Collider other)
        {
            OnEventTriggerEnter?.Invoke(other);
        }


        private void OnTriggerExit(Collider other)
        {
            OnEventTriggerExit?.Invoke(other);
        }

        private void OnCollisionEnter(UnityEngine.Collision other)
        {
            OnCollision?.Invoke(other);
        }
    }
}
