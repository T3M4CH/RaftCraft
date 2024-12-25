using Game.Scripts.Joystick;
using Game.Scripts.Joystick.Interfaces;
using Game.Scripts.Player.EntityGame;
using Game.Scripts.Player.StateMachine;
using Game.Scripts.Raft.Components;
using UnityEngine;

namespace Game.Scripts.Player.PlayerController.PlayerStates
{
    public class PlayerLadderMove : EntityState
    {
        private LadderObject _currentLadder;

        private EntityPlayer Player => (EntityPlayer)Entity;
        
        private readonly int _hasIsGround = Animator.StringToHash("IsGround");
        private readonly int _hasIsLadder = Animator.StringToHash("IsLadder");
        private readonly int _hasSpeedLadder = Animator.StringToHash("LadderSpeed");

        private readonly IJoystickService _joystick;
        private float _progressLeader;
        private float _directionMoveVertical;

        private float ProgressLadder
        {
            get => _progressLeader;

            set
            {
                _progressLeader = Mathf.Clamp(value, 0f, 1f);
            }
        }
        
        public PlayerLadderMove(EntityStateMachine stateMachine, Entity entity, IJoystickService joystickService) : base(stateMachine, entity)
        {
            _joystick = joystickService;
        }

        public void SetLadder(LadderObject ladder)
        {
            _currentLadder = ladder;
        }

        public override void Update()
        {
            _directionMoveVertical = _joystick.Direction.z;
        }
        
        public override void FixedUpdate()
        {
            var deltaTime = Time.fixedDeltaTime;
            if (_currentLadder == null) return;

            ProgressLadder += _directionMoveVertical * (deltaTime * Player.Settings.SpeedMoveLadder);
            var position = Vector3.Lerp(Player.transform.position, CurrentPositionLeader(), 15f * deltaTime);
            Player.Animator.SetFloat(_hasSpeedLadder, _joystick.Direction.z);
            Rotate(deltaTime);
            Player.Rb.MovePosition(position);
            
            if (ProgressLadder >= 1f || ProgressLadder <= 0f)
            {
                stateMachine.SetState<PlayerPlotState>();
            }
        }

        private void Rotate(float deltaTime)
        {
            var targetRotate = Quaternion.LookRotation(Vector3.forward);
            Entity.Components.ModelObject.rotation = Quaternion.RotateTowards(Entity.Components.ModelObject.rotation,
                targetRotate,
                360f * deltaTime * Player.Settings.SpeedRotateRaft);
        }

        private Vector3 CurrentPositionLeader()
        {
            return Vector3.Lerp(_currentLadder.StartPoint.position, _currentLadder.EndPoint.position, ProgressLadder);
        }
        
        public override void Enter()
        {
            base.Enter();
            _joystick.OnChangeDirection += JoystickOnOnChangeDirection;
            _progressLeader = Player.transform.position.y < _currentLadder.transform.position.y ? 0f : 1f;
            Player.Rb.useGravity = false;
            Player.Rb.isKinematic = true;
            Player.Animator.SetBool(_hasIsGround, false);
            Player.Animator.SetBool(_hasIsLadder, true);
        }

        private void JoystickOnOnChangeDirection(DirectionJoystick direction)
        {
            if (direction == DirectionJoystick.Left || direction == DirectionJoystick.Right)
            {
                if (ProgressLadder <= 0.2f)
                {
                    stateMachine.SetState<PlayerPlotState>();
                }
            }
        }

        public override void Exit()
        {
            base.Exit();
            _joystick.OnChangeDirection -= JoystickOnOnChangeDirection;
            Player.Animator.SetBool(_hasIsGround, true);
            Player.Animator.SetBool(_hasIsLadder, false);
            Player.Rb.useGravity = true;
            Player.Rb.isKinematic = false;
            _currentLadder = null;
        }
    }
}