using System;
using UnityEngine;

namespace Game.Scripts.Player.WeaponController.AnimationAttack
{
    public class AnimationAttackComponent : MonoBehaviour
    {
        [field: SerializeField] public string AnimationTrigger { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        private int _hashSpeedRun;
        public event Action OnStartAttackEvent;
        public event Action OnAttackEvent;
        public event Action OnEndAttackEvent;

        private void Awake()
        {
            _hashSpeedRun = Animator.StringToHash(AnimationTrigger);
        }

        public void StartAttack()
        {
            Animator.SetTrigger(_hashSpeedRun);
        }
        
        public void OnStartAttack()
        {
            OnStartAttackEvent?.Invoke();
        }

        public void OnEventAttack()
        {
            OnAttackEvent?.Invoke();
        }


        public void OnEndAttack()
        {
            OnEndAttackEvent?.Invoke();
        }
    }
}
