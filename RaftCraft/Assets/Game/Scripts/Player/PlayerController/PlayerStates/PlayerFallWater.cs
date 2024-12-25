using Game.Scripts.DownDoor;
using Game.Scripts.Player.EntityGame;
using Game.Scripts.Player.StateMachine;
using UnityEngine;

namespace Game.Scripts.Player.PlayerController.PlayerStates
{
    public class PlayerFallWater : EntityState
    {
        private DownDoorObject _door;
        private EntityPlayer Player => (EntityPlayer)Entity;
        private readonly int _hasFall = Animator.StringToHash("Fall");
        private readonly int _hasWater = Animator.StringToHash("SpawnedInWater");
        private readonly int _hasGround = Animator.StringToHash("IsGround");
        private float _timeFall;

        public PlayerFallWater(EntityStateMachine stateMachine, Entity entity) : base(stateMachine, entity)
        {
        }

        public void SetFall(DownDoorObject doorObject)
        {
            _door = doorObject;
        }

        public override void Enter()
        {
            base.Enter();
            _door.Enter(Player);
            _timeFall = 2f;
            Player.Rb.mass = 100;
            Player.Rb.drag = 0f;
            Player.Components.ModelObject.rotation = Quaternion.Euler(0f, 180f, 0f);
            Player.Animator.SetBool(_hasFall, true);
            Player.Animator.SetBool(_hasGround, false);
        }

        public override void Exit()
        {
            base.Exit();
            _door.Exit(Player);
            _timeFall = 2f;
            Player.Rb.mass = 5;
            Player.Rb.drag = 1f;
            Player.Animator.SetBool(_hasFall, false);
            Player.Animator.SetBool(_hasGround, false);
        }

        public override void Update()
        {
            base.Update();
            if (Player.transform.position.y <= -3f)
            {
                stateMachine.SetState<PlayerWaterState>();
            }
        }
    }
}