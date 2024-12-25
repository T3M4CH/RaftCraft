using System;
using Game.Scripts.DownDoor;
using Game.Scripts.Joystick;
using Game.Scripts.Joystick.Interfaces;
using Game.Scripts.Player.Collision;
using Game.Scripts.Player.EntityGame;
using Game.Scripts.Player.HeroPumping.Enums;
using Game.Scripts.Player.HeroPumping.Interfaces;
using Game.Scripts.Player.StateMachine;
using UnityEngine;

namespace Game.Scripts.Player.PlayerController.PlayerStates
{
    public class PlayerWaterState : EntityState, IUpdateRayCast, IDisposable
    {
        private IJoystickService _joystickService;
        private IPlayerUpgradeSettings _playerSettings;
        private EntityPlayer Player => (EntityPlayer)Entity;
        private int _hasSpeedMove = Animator.StringToHash("SpeedMove");
        private float _magnitude;
        private float _moveSpeedInWater;
        private Vector3 _directionMove;
        public PlayerWaterState(EntityStateMachine stateMachine, Entity entity, IJoystickService joystickService) : base(stateMachine, entity)
        {
            _playerSettings = Player.PlayerSettings;
            _playerSettings.OnUpgrade += ValidateSpeed;
            _joystickService = joystickService;
        }

        public override void Enter()
        {
            base.Enter();
            Player.Rb.useGravity = false;
            _moveSpeedInWater = _playerSettings.GetValue<float>(EPlayerUpgradeType.WaterSpeed);
            Player.RayCast.AddListener(this);
            //Player.Weapon.UseWeapon(0);
        }
        

        public override void Exit()
        {
            base.Exit();
            Player.EffectBubbleWater.SetActive(false);
            Player.Animator.SetFloat(_hasSpeedMove, 0f);
        }

        private Vector3 DirectionMoveWater()
        {
            var direction = _directionMove;
            if (Player.transform.position.y >= Player.Settings.MaxHeightMoveWater)
            {
                direction.y = Mathf.Clamp(direction.y, -1f, 0f);
            }

            if (Player.transform.position.x >= Player.Settings.MinMaxHorizontal.y)
            {
                direction.x = Mathf.Clamp(direction.x, -1f, 0f);
            }

            if (Player.transform.position.x <= Player.Settings.MinMaxHorizontal.x)
            {
                direction.x = Mathf.Clamp(direction.x, 0f, 1f);
            }

            return direction;
        }

        public override void Update()
        {
            _directionMove = new Vector3(_joystickService.Direction.x, _joystickService.Direction.z, 0f);
        }
        
        public override void FixedUpdate()
        {
            var deltaTime = Time.fixedDeltaTime;
            
            Player.EffectBubbleWater.SetActive(Player.transform.position.y <= -4f);
            Player.Rb.velocity = DirectionMoveWater() * (deltaTime * _moveSpeedInWater);
            _magnitude = _directionMove.magnitude > 0f
                ? Mathf.Clamp(_directionMove.magnitude, 0f, 1f)
                : Mathf.Lerp(_magnitude, 0f, deltaTime * Player.Settings.SpeedStopAnimation);
            Player.Animator.SetFloat(_hasSpeedMove, _magnitude);
            Rotate(deltaTime);
        }

        private Vector3 DirectionRotate()
        {
            Debug.Log($"Target: {Player.Weapon.HaveTarget}");
            if (Player.Weapon.HaveTarget == false)
            {
                return _directionMove;
            }

            var direction = (Player.Weapon.TargetTransform.position - Player.transform.position)
                .normalized;
            return direction;
        }

        private void Rotate(float deltaTime)
        {
            var direction = DirectionRotate();
            if (direction == Vector3.zero)
            {
                direction = Vector3.back;
            }

            var targetRotate = Quaternion.LookRotation(direction);
            Entity.Components.ModelObject.rotation = Quaternion.RotateTowards(Entity.Components.ModelObject.rotation, targetRotate,
                360f * deltaTime * Player.Settings.SpeedRotateWater);
        }

        public bool HaveListener
        {
            get => IsActive;
        }

        public void UpdateCast(CastData data)
        {
            if (data.target.CompareTag("Raft"))
            {
                var state = stateMachine.GetState<PlayerJumpToRaft>();
                state.SetTargetPosition(data.point);
                stateMachine.SetState<PlayerJumpToRaft>();
            }
        }

        private void ValidateSpeed(EPlayerUpgradeType upgradeType)
        {
            if (upgradeType == EPlayerUpgradeType.WaterSpeed)
            {
                _moveSpeedInWater = _playerSettings.GetValue<float>(EPlayerUpgradeType.WaterSpeed);
            }
        }

        public void Dispose()
        {
            _playerSettings.OnUpgrade -= ValidateSpeed;
        }
    }
}