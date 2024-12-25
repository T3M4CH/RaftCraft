using System;
using Game.Scripts.Joystick.Interfaces;
using Game.Scripts.Player.EntityGame;
using Game.Scripts.Player.PlayerController.PlayerStates;
using Game.Scripts.Player.StateMachine;
using UnityEngine;

namespace Game.Scripts.Player.WeaponController.WeaponState
{
    public class RaftWeaponState : EntityState
    {
        private WeaponController _weaponController;
        private IJoystickService _joystickService;

        private float _currentAngle;
        
        public RaftWeaponState(EntityStateMachine stateMachine, Entity entity, WeaponController weaponController, IJoystickService joystickService) : base(stateMachine, entity)
        {
            _weaponController = weaponController;
            _joystickService = joystickService;
        }

        public override void Enter()
        {
            base.Enter();
            _weaponController.Rigging.SetWeigh(1f);
            _weaponController.Rigging.SetWeightSpine(1f);
            Entity.StateMachine.OnEnterState += StateMachineOnOnEnterState;
            _currentAngle = 0f;
        }

        public bool HaveShot()
        {
            return _currentAngle >= 0.9f;
        }

        public override void Update()
        {
            _currentAngle = Mathf.Lerp(_currentAngle, _weaponController.HaveTarget ? 1f : _joystickService.Direction.magnitude, Time.smoothDeltaTime * 5f);
            _weaponController.WeaponHolder.localRotation = Quaternion.Euler(Vector3.Lerp(_weaponController.RotationHolderIdle, _weaponController.RotationHolderMove, _currentAngle));
        }

        private void StateMachineOnOnEnterState(EntityState state, Entity entity)
        {
            switch (state)
            {
                case PlayerDeathInGround playerDeathInGround:
                    stateMachine.SetState<IdleWeaponState>();
                    break;
                case PlayerDeathInWater playerDeathInWater:
                    stateMachine.SetState<IdleWeaponState>();
                    break;
                case PlayerFallState playerFallState:
                    break;
                case PlayerJumpToRaft playerJumpToRaft:
                    stateMachine.SetState<IdleWeaponState>();
                    break;
                case PlayerJumpToWater playerJumpToWater:
                    stateMachine.SetState<IdleWeaponState>();
                    break;
                case PlayerLadderMove playerLadderMove:
                    stateMachine.SetState<IdleWeaponState>();
                    break;
                case PlayerPlotState playerPlotState:
                    break;
                case PlayerWaterState playerWaterState:
                    break;
                case PlayerFallWater fallWater:
                    stateMachine.SetState<IdleWeaponState>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state));
            }
        }

        public override void Exit()
        {
            base.Exit();
            Entity.StateMachine.OnEnterState -= StateMachineOnOnEnterState;
        }
    }
}
