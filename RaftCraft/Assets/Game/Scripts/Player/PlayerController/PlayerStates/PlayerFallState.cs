using Game.Scripts.Player.Collision;
using Game.Scripts.Player.EntityGame;
using Game.Scripts.Player.StateMachine;
using Game.Scripts.Raft.Components;
using UnityEngine;

namespace Game.Scripts.Player.PlayerController.PlayerStates
{
    public class PlayerFallState : EntityState, IUpdateRayCast
    {
        private EntityPlayer Player => (EntityPlayer)Entity;
        private readonly int _hasFall = Animator.StringToHash("Fall");
        private FallObject _fall;
        
        public PlayerFallState(EntityStateMachine stateMachine, Entity entity) : base(stateMachine, entity)
        {
        }

        public void SetFall(FallObject fallObject)
        {
            _fall = fallObject;
        }
        
        public override void Enter()
        {
            base.Enter();
            Player.Rb.mass = 100;
            Player.Rb.drag = 0f;
            Player.RayCast.AddListener(this);
            Player.Animator.SetBool(_hasFall, true);
            Player.Components.Collision.OnEventTriggerExit += CollisionOnOnEventTriggerExit;
            if (_fall != null)
            {
                _fall.Enter(Player);
            }
        }

        private void CollisionOnOnEventTriggerExit(Collider collider)
        {
            if (collider.TryGetComponent<FallObject>(out var fall))
            {
                fall.Exit(Player);
                stateMachine.SetState<PlayerPlotState>();
            }
        }

        public override void Update()
        {
            if (_fall == null) return;

            if (_fall.transform.position.y - 1.5f > Player.transform.position.y)
            {
                _fall.Exit(Player);
                stateMachine.SetState<PlayerPlotState>();
            }
        }

        public override void Exit()
        {
            base.Exit();
            Player.Rb.mass = 5;
            Player.Rb.drag = 1f;
            Player.Components.Collision.OnEventTriggerExit -= CollisionOnOnEventTriggerExit;
            Player.Animator.SetBool(_hasFall, false);
        }

        public bool HaveListener => IsActive;
        public void UpdateCast(CastData data)
        {
            if (_fall != null)
            {
                return;
            }
            if (data.target.CompareTag("Raft"))
            {
                var dist = Vector3.Distance(Player.transform.position, data.point);
                if (dist <= 1.5f)
                {
                    if (_fall != null)
                    {
                        _fall.Exit(Player);
                    }
                    stateMachine.SetState<PlayerPlotState>();
                }
            }
        }
    }
}