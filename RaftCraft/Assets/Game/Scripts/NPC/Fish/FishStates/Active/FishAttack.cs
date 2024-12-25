using Game.Scripts.NPC.Fish.FishStates.Passive;
using Game.Scripts.Player.EntityGame;
using Game.Scripts.Player.StateMachine;
using Game.Scripts.Player.WeaponController.AnimationAttack;
using UnityEngine;

namespace Game.Scripts.NPC.Fish.FishStates.Active
{
    public class FishAttack : FishState
    {
        protected readonly int AttackHash = Animator.StringToHash("Attack");

        private AnimationAttackComponent CurrentAttack =>
            currentBaseFish.ComponentsFish.AttackComponents[currentBaseFish.Data.Level];

        private Entity _target;
        
        public FishAttack(EntityStateMachine stateMachine, Entity entity) : base(stateMachine, entity)
        {
        }

        public override void Enter()
        {
            base.Enter();
            CurrentAttack.StartAttack();
            CurrentAttack.OnAttackEvent += CurrentAttackOnOnAttackEvent;
            CurrentAttack.OnEndAttackEvent += CurrentAttackOnOnEndAttackEvent;
        }

        public override void Exit()
        {
            base.Exit();
            CurrentAttack.OnAttackEvent -= CurrentAttackOnOnAttackEvent;
            CurrentAttack.OnEndAttackEvent -= CurrentAttackOnOnEndAttackEvent;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (_target == null)
            {
                return;
            }

            LookAt((_target.transform.position - currentBaseFish.Components.ParentObject.position).normalized);
        }

        private void LookAt(Vector3 target)
        {
            if (target == Vector3.zero) return;
        
            var targetRotation = Quaternion.LookRotation(target);
            targetRotation = Quaternion.RotateTowards( currentBaseFish.Components.ModelObject.rotation, targetRotation, currentBaseFish.Data.SpeedRotate * Time.fixedDeltaTime);
            currentBaseFish.Components.ModelObject.rotation = targetRotation;
        }

        public void SetTarget(Entity target)
        {
            _target = target;
        }

        private void CurrentAttackOnOnEndAttackEvent()
        {
            stateMachine.SetState<FishIdle>();
        }

        private float DistanceTarget()
        {
            return Vector3.Distance(currentBaseFish.Components.ParentObject.position, _target.transform.position);
        }
        
        private void CurrentAttackOnOnAttackEvent()
        {
            if (DistanceTarget() <= currentBaseFish.Data.DistanceAttack)
            {
                if (_target is IDamagable damagable)
                {
                    damagable.TakeDamage(currentBaseFish.Data.Damage, Entity.transform.position);
                }
            }
        }
    }
}
