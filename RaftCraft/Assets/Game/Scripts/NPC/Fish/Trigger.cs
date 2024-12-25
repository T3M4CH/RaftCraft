using System;
using UnityEngine;

namespace Game.Scripts.NPC
{
    public class Trigger : MonoBehaviour
    {
        public event Action<Collider> OnTriggerBegin; 

        public event Action<Collider> OnTriggerRelease;
        
        public Vector3 CurrentPositionObject { get; private set; } 

        private void OnTriggerEnter(Collider col)
        {
            OnTriggerBegin?.Invoke(col);
        }
        
        private void OnTriggerStay(Collider col)
        {
            CurrentPositionObject = col.transform.position;
        }
        
        private void OnTriggerExit(Collider col)
        {
            OnTriggerRelease?.Invoke(col);
        }
    }
}