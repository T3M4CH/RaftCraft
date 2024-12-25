using Game.Scripts.NPC.Fish.FishStates.Active;
using Game.Scripts.NPC.Interface;
using Game.Scripts.Player.EntityGame;
using Game.Scripts.Player.StateMachine;
using UnityEngine;

namespace Game.Scripts.NPC.Fish.FishStates.Passive
{
    public class FishIdle : FishState
    {
        private float _timeIdle;
        private bool _runAway;
        private bool _haveAttack;
        
        public FishIdle(EntityStateMachine stateMachine, Entity entity, bool runAway = false, bool attack = false) : base(stateMachine, entity)
        {
            _runAway = runAway;
            _haveAttack = attack;
        }

        public override void Enter()
        {
            base.Enter();
            currentBaseFish.ComponentsFish.EventTarget.OnTriggerEnterEvent += EventTargetOnOnTriggerEnterEvent;
            currentBaseFish.ComponentsFish.EventTarget.OnTriggerStayEvent += EventTargetOnOnTriggerEnterEvent;
            CurrentAnimator.SetFloat(HashSpeedRun, Random.Range(0.9f, 1.1f));
            _timeIdle = currentBaseFish.Data.TimeStayIdle;
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

        public override void Exit()
        {
            base.Exit();
            currentBaseFish.ComponentsFish.EventTarget.OnTriggerEnterEvent -= EventTargetOnOnTriggerEnterEvent;
            currentBaseFish.ComponentsFish.EventTarget.OnTriggerStayEvent -= EventTargetOnOnTriggerEnterEvent;
        }

        public override void Update()
        {
            base.Update();
            _timeIdle -= Time.smoothDeltaTime;
            if (_timeIdle <= 0f)
            {
                stateMachine.SetState<FishMove>();
                currentBaseFish.ComponentsFish.Seeker.StartPath(currentBaseFish.Components.ParentObject.position,
                    GetRandomPointMove());
            }
        }

        private Vector3 GetRandomPointMove()
        {
            var fish = currentBaseFish;

            if (fish.YSpawnPosition > fish.YBarrier)
            {
                return fish.Habitat.GetNewTargetPoint(currentBaseFish.Components.ParentObject.position, fish.YBarrier, fish.YMax);
            }

            return fish.Habitat.GetNewTargetPoint(currentBaseFish.Components.ParentObject.position, fish.YMin, fish.YBarrier);
        }
    }
}
