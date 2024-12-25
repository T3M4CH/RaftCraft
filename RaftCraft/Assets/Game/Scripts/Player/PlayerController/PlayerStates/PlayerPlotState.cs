using System;
using Game.Scripts.BattleMode;
using Game.Scripts.DownDoor;
using Game.Scripts.Joystick;
using Game.Scripts.Joystick.Interfaces;
using Game.Scripts.Player.Collision;
using Game.Scripts.Player.HeroPumping.Enums;
using Game.Scripts.Player.HeroPumping.Interfaces;
using Game.Scripts.Player.StateMachine;
using Game.Scripts.Raft.Components;
using Game.Scripts.Raft.Interface;
using UnityEngine;

namespace Game.Scripts.Player.PlayerController.PlayerStates
{
    public class PlayerPlotState : EntityState, IUpdateRayCast, IDisposable
    {
        public event Action<MovementState> OnChangeMovement; 
        
        private readonly int _hasSpeedMove = Animator.StringToHash("SpeedMove");

        private readonly IPlayerUpgradeSettings _playerSettings;
        private readonly IJoystickService _joystickService;
        private readonly IBattleService _battleService;
        private readonly IRaftStructures _raftStructures;
        
        private Vector3 _currentRotateDirectionVector;
        private float _magnitude;
        private float _moveSpeedRaft;
        private bool _haveJumped;
        private Vector3 _lastPosition;
        private float _directionMoveHorizontal;

        private LadderObject _currentLadder;
        private FallObject _fall;
        private DownDoorObject _doorObject;
        
        private MovementState _stateMove;

        private MovementState StateMove
        {
            get => _stateMove;

            set
            {
                if (_stateMove == value)
                {
                    return;
                }

                _stateMove = value;
                OnChangeMovement?.Invoke(_stateMove);
            }
        }
        
        public PlayerPlotState(EntityStateMachine stateMachine, EntityPlayer entity, IJoystickService joystickService, IBattleService battleService, IRaftStructures raftStructures) :
            base(stateMachine, entity)
        {
            _raftStructures = raftStructures;
            _battleService = battleService;
            _playerSettings = entity.PlayerSettings;
            _playerSettings.OnUpgrade += ValidateSpeed;
            _moveSpeedRaft = _playerSettings.GetValue<float>(EPlayerUpgradeType.RaftSpeed);
            _currentRotateDirectionVector = RotateDirectionToVector3(RotateDirection.Right);
            _joystickService = joystickService;
            _haveJumped = true;
        }

        private EntityPlayer Player => (EntityPlayer)Entity;

        public bool HaveListener => IsActive;

        public bool HaveMove => DirectionMove(_joystickService.Direction) != 0f;

        public void UpdateCast(CastData data)
        {
            if (_haveJumped == false)
            {
                return;
            }
            
            if (data.target.CompareTag("Water"))
            {
                stateMachine.SetState<PlayerJumpToWater>();
            }
        }

        public override void Enter()
        {
            base.Enter();
            _fall = null;
            _currentLadder = null;
            _joystickService.OnChangeDirection += JoystickServiceOnOnChangeDirection;
            Player.Components.Collision.OnEventTriggerEnter += CollisionOnOnEventTriggerEnter;
            Player.Components.Collision.OnEventTriggerExit += CollisionOnOnEventTriggerExit;
            _battleService.OnChangeState += OnChangeEventBattle;
            if (_battleService.CurrentState == BattleState.Idle)
            {
                Debug.Log($"Set state jumped true");
                _haveJumped = true;
            }
            Player.RayCast.AddListener(this);
            Player.Weapon.UseSelected();
            _directionMoveHorizontal = 0f;
        }

        private void JoystickServiceOnOnChangeDirection(DirectionJoystick direction)
        {
            switch (direction)
            {
                case DirectionJoystick.Up:
                {
                    if (_currentLadder != null && _currentLadder.transform.position.y >= Player.transform.position.y)
                    {
                        stateMachine.GetState<PlayerLadderMove>().SetLadder(_currentLadder);
                        stateMachine.SetState<PlayerLadderMove>();
                    }

                    break;
                }
                case DirectionJoystick.Down:
                {
                    if (_currentLadder != null && _currentLadder.transform.position.y <= Player.transform.position.y)
                    {
                        stateMachine.GetState<PlayerLadderMove>().SetLadder(_currentLadder);
                        stateMachine.SetState<PlayerLadderMove>();
                        _doorObject = null;
                        break;
                    }

                    if (_fall != null)
                    {
                        stateMachine.GetState<PlayerFallState>().SetFall(_fall);
                        stateMachine.SetState<PlayerFallState>();
                        var targetRotate = Quaternion.LookRotation(_currentRotateDirectionVector);
                        Entity.Components.ModelObject.rotation = targetRotate;
                        _doorObject = null;
                        break;
                    }

                    if (_doorObject != null && _doorObject.Interaction)
                    {
                        if (Vector3.Distance(Player.transform.position, _doorObject.transform.position) <= 1f)
                        {
                            stateMachine.GetState<PlayerFallWater>().SetFall(_doorObject);
                            stateMachine.SetState<PlayerFallWater>();
                            _doorObject = null;
                            break;
                        }
                    }

                    break;
                }
            }
        }

        private void CollisionOnOnEventTriggerExit(Collider collider)
        {
            if (collider.TryGetComponent<LadderObject>(out var ladder))
            {
                _currentLadder = null;
            }
            
            if (collider.TryGetComponent<FallObject>(out var fall))
            {
                _fall = null;
            }
            
            if (collider.TryGetComponent<DownDoorObject>(out var door))
            {
                _doorObject = null;
            }
        }

        private void CollisionOnOnEventTriggerEnter(Collider collider)
        {
            if (collider.TryGetComponent<LadderObject>(out var ladder))
            {
                _currentLadder = ladder;
            }

            if (collider.TryGetComponent<FallObject>(out var fall))
            {
                _fall = fall;
            }

            if (collider.TryGetComponent<DownDoorObject>(out var door) && _fall == null && _currentLadder == null)
            {
                _doorObject = door;
            }
        }

        private void OnChangeEventBattle(BattleState state)
        {
            switch (state)
            {
                case BattleState.СutScene:
                    _joystickService.HideGUI();
                    _haveJumped = false;
                    break;
                case BattleState.Fight:
                    _joystickService.ReturnGUI();
                    _joystickService.HideGUI(false);
                    break;
                case BattleState.Idle:
                    _haveJumped = true;
                    break;
            }
        }

        public void SetDirectionRotate(Vector3 endPoint)
        {
            var rotateDirection = endPoint.x >= Player.Components.ParentObject.position.x
                ? RotateDirection.Left
                : RotateDirection.Right;
            _currentRotateDirectionVector = RotateDirectionToVector3(rotateDirection);
        }


        public override void Exit()
        {
            base.Exit();
            Player.Rb.velocity = Vector3.zero;
            _doorObject = null;
            _joystickService.OnChangeDirection -= JoystickServiceOnOnChangeDirection;
            Player.Components.Collision.OnEventTriggerExit -= CollisionOnOnEventTriggerExit;
            Player.Components.Collision.OnEventTriggerEnter -= CollisionOnOnEventTriggerEnter;
            _battleService.OnChangeState -= OnChangeEventBattle;
            Player.Animator.SetFloat(_hasSpeedMove, 0f);
        }

        public override void Update()
        {
            _directionMoveHorizontal = DirectionMove(_joystickService.Direction);
        }
        
        public override void FixedUpdate()
        {
            if(Player.Rb.isKinematic) return;
            
            var deltaTime = Time.fixedDeltaTime;
            
            var velocity = Player.Rb.velocity;
            velocity.x = _directionMoveHorizontal * (deltaTime * _moveSpeedRaft);
            Player.Rb.velocity = velocity;
            
            _magnitude = Mathf.Abs(_directionMoveHorizontal) > 0f
                ? Mathf.Clamp(Mathf.Abs(_directionMoveHorizontal), 0f, 1f)
                : Mathf.Lerp(_magnitude, 0f, deltaTime * Player.Settings.SpeedStopAnimation);

            if (Player.Weapon.HaveTarget == false)
            {
                if (_directionMoveHorizontal != 0f)
                {
                    _currentRotateDirectionVector = _directionMoveHorizontal switch
                    {
                        > 0f => RotateDirectionToVector3(RotateDirection.Right),
                        < 0f => RotateDirectionToVector3(RotateDirection.Left),
                        _ => _currentRotateDirectionVector
                    };
                }
            }
            else
            {
                _currentRotateDirectionVector = VectorTarget();
            }

            Rotate(deltaTime);
            Player.Animator.SetFloat(_hasSpeedMove, MagnitudeMove(_magnitude));
            StateMove = HaveMove ? MovementState.Move : MovementState.Idle;
        }
        
        
        private void ValidateSpeed(EPlayerUpgradeType upgradeType)
        {
            if (upgradeType == EPlayerUpgradeType.RaftSpeed)
            {
                _moveSpeedRaft = _playerSettings.GetValue<float>(EPlayerUpgradeType.RaftSpeed);
            }
        }

        public void Dispose()
        {
            _playerSettings.OnUpgrade -= ValidateSpeed;
        }

        private float MagnitudeMove(float magnitude)
        {
            var direction = (_lastPosition - Player.transform.position).normalized;
            _lastPosition = Player.transform.position;
            var moveDirection = (Player.Components.ModelObject.forward - direction).normalized;
            return moveDirection.x != 0f ? magnitude : -magnitude;
        }
        
        private Vector3 RotateDirectionToVector3(RotateDirection rotateDirection)
        {
            return rotateDirection == RotateDirection.Left ? Vector3.left : Vector3.right;
        }

        private Vector3 VectorTarget()
        {
            return Player.Weapon.TargetPosition.x > Player.transform.position.x ? Vector3.right : Vector3.left;
        }

        private void Rotate(float deltaTime)
        {
            var targetRotate = Quaternion.LookRotation(_currentRotateDirectionVector);
            Entity.Components.ModelObject.rotation = Quaternion.RotateTowards(Entity.Components.ModelObject.rotation,
                targetRotate,
                360f * deltaTime * Player.Settings.SpeedRotateRaft);
        }

        private float DirectionMove(Vector3 joystickDirection)
        {
            return _haveJumped ? joystickDirection.x : _raftStructures.ClampPositionHorizontal(Player.transform.position, joystickDirection.x, 0.3f);
        }

        private enum RotateDirection
        {
            Right,
            Left
        }
    }
}