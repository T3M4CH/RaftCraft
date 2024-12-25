using Game.Scripts.Player.EntityGame;
using Game.Scripts.Player.StateMachine;
using UnityEngine;

namespace Game.Scripts.Player.PlayerController.PlayerStates
{
    public class PlayerJumpToRaft : EntityState
    {
        private int _hasJump = Animator.StringToHash("IsGround");
        private EntityPlayer Player => (EntityPlayer)Entity;
        private Vector3 _startPosition;
        private Vector3 _endPosition;
        private float _progress;
        
        public PlayerJumpToRaft(EntityStateMachine stateMachine, Entity entity) : base(stateMachine, entity)
        {
        }

        public override void FixedUpdate()
        {
            var deltaTime = Time.fixedDeltaTime;
            var position = Vector3.Lerp(_startPosition, _endPosition, _progress);
            position.y += Player.Settings.CurveJumpRaft.Evaluate(_progress) * Player.Settings.HeightJumpToRaft;
            _progress += deltaTime * Player.Settings.SpeedJumpToRaft;
            Player.Rb.MovePosition(position);
            
            if (_progress >= 1f)
            {
                Player.Character.localEulerAngles = Vector3.zero;
                var state = stateMachine.GetState<PlayerPlotState>();
                state.SetDirectionRotate(_startPosition);
                stateMachine.SetState<PlayerPlotState>();
            }
        }

        public void SetTargetPosition(Vector3 target)
        {
            _endPosition = target;
            _endPosition.z = 0f;
        }

        public override void Enter()
        {
            Player.MainCollider.enabled = false;
            Player.Rb.interpolation = RigidbodyInterpolation.None;
            _startPosition = Player.transform.position;
            _startPosition.z = 0f;
            _progress = 0f;
            Entity.Components.ModelObject.LookAt(Vector3.right);
            Player.Animator.SetBool(_hasJump, true);
        }

        public override void Exit()
        {
            Player.Rb.interpolation = RigidbodyInterpolation.Interpolate;
            Player.Rb.useGravity = true;
            Player.MainCollider.enabled = true;
        }
    }
}
