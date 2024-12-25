using Game.Scripts.NPC.Fish.FishStates.Passive;
using Game.Scripts.NPC.Interface;
using Game.Scripts.Player.EntityGame;
using Game.Scripts.Player.StateMachine;
using UnityEngine;

namespace Game.Scripts.NPC.Fish.FishStates.Active
{
    public class FishMoveAttackState : FishState
    {
        private Entity _target;
        private ISpace _targetSpace;
        public FishMoveAttackState(EntityStateMachine stateMachine, Entity entity) : base(stateMachine, entity)
        {
       
        }


        public override void Enter()
        {
            base.Enter();
            currentBaseFish.ComponentsFish.EventTarget.OnTriggerExitEvent += EventTargetOnOnTriggerExitEvent;
        }

        private void EventTargetOnOnTriggerExitEvent(Collider target)
        {
            if (target.TryGetComponent<Entity>(out var entity))
            {
                if (_target != entity)
                {
                    return;
                }

                _target = null;
                stateMachine.SetState<FishIdle>();
            }
        }

        public override void Exit()
        {
            base.Exit();
            currentBaseFish.ComponentsFish.EventTarget.OnTriggerExitEvent -= EventTargetOnOnTriggerExitEvent;
        }

        public void SetTargetAttack(Entity target)
        {
            _target = target;
            if (_target is ISpace space)
            {
                _targetSpace = space;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Move();
        }
        
        private void Move()
        {
            if (_target == null)
            {
                return;
            }

            var target = (_target.Components.ParentObject.position - currentBaseFish.Components.ParentObject.position).normalized;
            currentBaseFish.ComponentsFish.Rb.MovePosition(currentBaseFish.Components.ParentObject.position + (target * (Time.fixedDeltaTime * currentBaseFish.Data.SpeedMoveAttack)));
            LookAt(target);
            if (DistanceTarget() <= currentBaseFish.Data.DistanceAttack || (_targetSpace != null && _targetSpace.Space == LocationSpace.Ground))
            {
                stateMachine.GetState<FishAttack>().SetTarget(_target);
                stateMachine.SetState<FishAttack>();
            }
            
        }

        private float DistanceTarget()
        {
            return Vector3.Distance(currentBaseFish.Components.ParentObject.position, _target.transform.position);
        }
        
        private void LookAt(Vector3 target)
        {
            if (target == Vector3.zero) return;
        
            var targetRotation = Quaternion.LookRotation(target);
            targetRotation = Quaternion.RotateTowards( currentBaseFish.Components.ModelObject.rotation, targetRotation, currentBaseFish.Data.SpeedRotate * Time.fixedDeltaTime);
            currentBaseFish.Components.ModelObject.rotation = targetRotation;
        }
    }
}
