using System;
using UnityEngine;

namespace Game.Scripts.Utils
{
    public class AnimatorHelperEvent : MonoBehaviour
    {
        public event Action OnAnimationPunchEvent;

        public event Action OnAnimationDeathEvent;
        public void PunchEvent()
        {
            OnAnimationPunchEvent?.Invoke();
        }
        
        public void DeathEvent()
        {
            OnAnimationDeathEvent?.Invoke();
        }
    }
}