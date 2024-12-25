using System.Collections.Generic;
using Game.Scripts.NPC.Fish.FishStates.Active;
using Game.Scripts.NPC.Interface;
using Game.Scripts.Player.EntityGame;
using Game.Scripts.Player.StateMachine;
using Pathfinding;
using UnityEngine;

namespace Game.Scripts.NPC.Fish.FishStates.Passive
{
    public class FishMove : FishState
    {
        private List<Vector3> _path = new List<Vector3>();
        private bool _runAway;
        private bool _haveAttack;
        public FishMove(EntityStateMachine stateMachine, Entity entity, bool runAway = false, bool attack = false) : base(stateMachine, entity)
        {
            _runAway = runAway;
            _haveAttack = attack;
        }

        public override void Enter()
        {
            base.Enter();
            //TODO: Отключил
            //CurrentAnimator.SetTrigger(RunHash);
            currentBaseFish.ComponentsFish.Seeker.pathCallback += PathCallback;
            currentBaseFish.ComponentsFish.EventTarget.OnTriggerEnterEvent += EventTargetOnOnTriggerEnterEvent;
            currentBaseFish.ComponentsFish.EventTarget.OnTriggerStayEvent += EventTargetOnOnTriggerEnterEvent;
        }

        private void EventTargetOnOnTriggerEnterEvent(Collider collider)
        {
            if (collider.TryGetComponent<Entity>(out var entity) == false)
            {
                return;
            }

            if ((entity is ISpace space) == false)
            {
                return;
            }

            if (space.Space == LocationSpace.Ground)
            {
                return;
            }

            stateMachine.GetState<FishMoveAway>().SetAway(collider.transform);
            stateMachine.SetState<FishMoveAway>();
        }

        private void PathCallback(Path path)
        {
            _path = path.vectorPath;
        }

        public override void Exit()
        {
            base.Exit();
            currentBaseFish.ComponentsFish.Seeker.pathCallback += PathCallback;
            currentBaseFish.ComponentsFish.EventTarget.OnTriggerEnterEvent -= EventTargetOnOnTriggerEnterEvent;
            currentBaseFish.ComponentsFish.EventTarget.OnTriggerStayEvent -= EventTargetOnOnTriggerEnterEvent;
        }
        
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Move();
        }

        private void Move()
        {
            if (HaveUpdateNextPoint())
            {
                return;
            }

            var target = ( GetNextPoint() - currentBaseFish.Components.ParentObject.position).normalized;
            
            if (currentBaseFish.YSpawnPosition > currentBaseFish.YBarrier)
            {
                target.y = Mathf.Max(target.y, currentBaseFish.YBarrier + 2);
            }
            else
            {
                target.y = Mathf.Min(target.y, currentBaseFish.YBarrier - 2);
            }
            
            currentBaseFish.ComponentsFish.Rb.MovePosition(currentBaseFish.Components.ParentObject.position + (target * (Time.fixedDeltaTime * currentBaseFish.Data.SpeedMove)));
            LookAt(target);

            if (HaveCompletePath())
            {
                stateMachine.SetState<FishIdle>();
            }
            
        }

        private Vector3 GetNextPoint()
        {
            if (_path == null || _path.Count == 0)
            {
                return currentBaseFish.Components.ParentObject.position;
            }

            return _path[0];
        }

        private bool HaveCompletePath()
        {
            return _path.Count == 0;
        }

        private bool HaveUpdateNextPoint()
        {
            if (_path == null || _path.Count == 0)
            {
                return false;
            }

            if (!(Vector3.Distance(currentBaseFish.Components.ParentObject.position, _path[0]) <=
                  currentBaseFish.Data.DistanceUpdatePatch)) return false;
            _path.RemoveAt(0);
            return true;

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
