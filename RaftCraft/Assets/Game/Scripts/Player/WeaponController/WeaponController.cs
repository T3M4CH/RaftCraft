using System;
using System.Collections.Generic;
using Game.Scripts.Core.Interface;
using Game.Scripts.Joystick.Interfaces;
using Game.Scripts.NPC;
using Game.Scripts.NPC.Fish;
using Game.Scripts.Player.EntityGame;
using Game.Scripts.Player.HeroPumping.Enums;
using Game.Scripts.Player.PlayerController.PlayerStates;
using Game.Scripts.Player.RigginController;
using Game.Scripts.Player.StateMachine;
using Game.Scripts.Player.WeaponController.AnimationAttack;
using Game.Scripts.Player.WeaponController.WeaponInventory;
using Game.Scripts.Player.WeaponController.WeaponState;
using Game.Scripts.Saves;
using Game.Scripts.Triggers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Player.WeaponController
{
    public class WeaponController : MonoBehaviour, IGameObserver<WeaponItem>
    {
        [field: SerializeField, FoldoutGroup("Components")]
        public Transform Aim { get; private set; }
        
        [field: SerializeField, FoldoutGroup("Components")]
        public HumanoidRiggingWeapon Rigging { get; private set; }

        [field: SerializeField, FoldoutGroup("Components")]
        public EntityPlayer Entity { get; private set; }

        [field: SerializeField, FoldoutGroup("Components")]
        public CollisionEvent Collision { get; private set; }

        [field: SerializeField, FoldoutGroup("Components")]
        public EntityType TargetEntity { get; private set; }
        
        [field: SerializeField, FoldoutGroup("Components")]
        public AnimationAttackComponent AttackComponent { get; private set; }
        [field: SerializeField] public Vector3 RotationHolderIdle { get; private set; }
        [field: SerializeField] public Vector3 RotationHolderMove { get; private set; }
        
        [SerializeField, FoldoutGroup("Entity Weapons")]
        private List<WeaponEntityData> _weaponData = new List<WeaponEntityData>();

        [field: SerializeField, FoldoutGroup("Entity Weapons")]
        public Transform WeaponHolder { get; private set; }

        [SerializeField, FoldoutGroup("Settings")] private Vector3 _defaultPosition;

        public bool HaveTarget
        {
            get
            {
                if (Target == null)
                {
                    return false;
                }
                else
                {
                    if (Target.Type != EntityType.Fish)
                    {
                        if (Target.HaveLife == false)
                        {
                            Target = null;
                            return false;
                        }
                        return true;
                    }
                    else
                    {
                        if (((BaseFish)Target).HaveProgress() == false)
                        {
                            return false;
                        }

                        return true;
                    }
                }
            }
        }

        public Transform TargetTransform => HaveTarget == false ? null : Target.transform;

        private EntityStateMachine _stateMachine;
        private IdleWeaponState _idleState;
        private RaftWeaponState _raftState;
        private GameSave _gameSave;
        private IGameObservable<WeaponUpgrade> _upgradeService;
        private IInventoryWeapon _inventoryWeapon;
        private IGameObservable<WeaponItem> _observableSelector;
        
        private Dictionary<WeaponId, Weapon> _weapons = new Dictionary<WeaponId, Weapon>();
        private Weapon _current;
        
        private Entity _target;

        private Entity Target
        {
            get => _target;

            set
            {
                _target = value;
                if (_target != null)
                {
                    Aim.position = TargetPosition;
                }
                else
                {
                    Aim.localPosition = _defaultPosition;
                }
            }
        }

        public Vector3 TargetPosition => HaveTarget == false ? Vector3.zero : ((IDamagable)Target).Position;

        private float _progressReload;
        #region Weapon Core

        private bool _haveReload;

        #endregion
        
        public void InitSystem(IJoystickService joystickService, IGameObservable<WeaponUpgrade> upgradeService, IInventoryWeapon inventoryWeapon, IGameObservable<WeaponItem> observableSelector)
        {
            _inventoryWeapon = inventoryWeapon;
            _upgradeService = upgradeService;
            _observableSelector = observableSelector;
            foreach (var data in _weaponData)
            {
                if (data.Behaviour != null)
                {
                    _upgradeService.AddObserver(data.Behaviour);
                }
                var weapon = new Weapon(data, Rigging);
                weapon.Hide();
                _weapons.Add(data.Behaviour.Id, weapon);
            }

            _haveReload = true;
            _stateMachine = new EntityStateMachine();
            _idleState = new IdleWeaponState(_stateMachine, Entity, this);
            _raftState = new RaftWeaponState(_stateMachine, Entity, this, joystickService);
            _stateMachine.AddState(_idleState);
            _stateMachine.AddState(_raftState);
            _stateMachine.OnEnterState += StateMachineOnOnEnterState;
            _stateMachine.SetState<IdleWeaponState>();
            _observableSelector.AddObserver(this);
        }

        private void StateMachineOnOnEnterState(EntityState state, Entity entity)
        {
            if (state is IdleWeaponState)
            {
                HideWeapon();
            }
        }

        public void UseSelected()
        {
            var last = _inventoryWeapon.GetCurrentSelected();
            if (last != null)
            {
                UseWeapon(last.id);
            }
        }

        [Button]
        public void UseWeapon(WeaponId id)
        {
            if (Entity.StateMachine == null)
            {
                return;
            }
            if (Entity.Space != LocationSpace.Ground)
            {
                return;
            }
            if (_weapons.ContainsKey(id) == false)
            {
                return;
            }

            if (_current != null)
            {
                _current.Hide();
            }
            _current = _weapons[id];
            _current.Use();
            switch (_current.WeaponEntityData.Type)
            {
                case WeaponType.Hand:
                    Collision.SetRadius(3f);
                    break;
                case WeaponType.Melee:
                    break;
                case WeaponType.Pistol:
                    _stateMachine.SetState<RaftWeaponState>();
                    Collision.SetRadius(_current.WeaponEntityData.Behaviour.DistanceAttack());
                    break;
                case WeaponType.Rifle:
                    _stateMachine.SetState<RaftWeaponState>();
                    Collision.SetRadius(_current.WeaponEntityData.Behaviour.DistanceAttack());
                    break;
                default:
                    _stateMachine.SetState<IdleWeaponState>();
                    break;
            }
        }

        [Button]
        private void Attack()
        {
            if (_current == null)
            {
                return;
            }

            if (_haveReload == false)
            {
                return;
            }

            switch (_current.WeaponEntityData.Type)
            {
                case WeaponType.Hand:
                    AttackComponent.StartAttack();
                    AttackComponent.OnAttackEvent += DamageTarget;
                    AttackComponent.OnEndAttackEvent += CompleteReload;
                    _haveReload = false;
                    break;
                case WeaponType.Melee:
                    break;
                case WeaponType.Pistol:
                    if (_current.WeaponEntityData.Behaviour == null)
                    {
                        return;
                    }
                    _current.WeaponEntityData.Behaviour.Shot(TargetEntity, Target);
                    _haveReload = false;
                    _progressReload = _current.WeaponEntityData.Behaviour.TimeReload();
                    break;
                case WeaponType.Rifle:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DamageTarget()
        {
            if (_current == null)
            {
                return;
            }

            if (HaveTarget == false)
            {
                return;
            }

            switch (_current.WeaponEntityData.Type)
            {
                case WeaponType.Hand:
                    AttackComponent.OnAttackEvent -= DamageTarget;
                    if (Target is IDamagable damage)
                    {
                        damage.TakeDamage(1, Entity.transform.position);
                    }
                    break;
                case WeaponType.Melee:
                    break;
                case WeaponType.Pistol:
                    break;
                case WeaponType.Rifle:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CompleteReload()
        {
            if (_current != null)
            {
                switch (_current.WeaponEntityData.Type)
                {
                    case WeaponType.Hand:
                        AttackComponent.OnAttackEvent -= DamageTarget;
                        AttackComponent.OnEndAttackEvent -= CompleteReload;
                        break;
                    case WeaponType.Melee:
                        break;
                    case WeaponType.Pistol:
                        break;
                    case WeaponType.Rifle:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _haveReload = true;
        }

        private void Update()
        {
            if (_stateMachine == null)
            {
                return;
            }
            
            _stateMachine.Update();

            if (Target != null && Target.gameObject.activeSelf == false)
            {
                Target = null;
            }
            if (_current == null)
            {
                return;
            }

            if (HaveTarget == false)
            {
                Target = null;
                return;
            }

            if (Target == null)
            {
                return;
            }
            
            if (Mathf.Abs(transform.position.y - Target.transform.position.y) > 3 && Target.Type != EntityType.Fish)
            {
                Target = null;
                return;
            }
            
            Aim.transform.position = TargetPosition;
            
            switch (_current.WeaponEntityData.Type)
            {
                case WeaponType.Hand:
                    if (_haveReload == false)
                    {
                        return;
                    }
                    if (Entity.StateMachine.CurrentEntityState is PlayerWaterState == false)
                    {
                        return;
                    }
                    
                    if (Target == null)
                    {
                        return;
                    }
                    if (Target.Type == EntityType.Fish)
                    {
                        return;
                    }
                    break;
                case WeaponType.Melee:
                    break;
                case WeaponType.Pistol:
                    if (_haveReload == false)
                    {
                        _progressReload -= Time.smoothDeltaTime;
                        if (_progressReload <= 0f)
                        {
                            _haveReload = true;
                        }
                        else
                        {
                            return;
                        }
                    }
                    if (Entity.StateMachine.CurrentEntityState is PlayerPlotState == false)
                    {
                        return;
                    }
                    
                    if (HaveAttack() && _stateMachine.GetState<RaftWeaponState>().HaveShot())
                    {
                        Attack();
                    }
                    break;
                case WeaponType.Rifle:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool HaveAttack()
        {
            if (_current == null)
            {
                return false;
            }

            return HaveTarget;
        }

        private void OnEnable()
        {
            Collision.OnTriggerEnterEvent += CollisionOnOnTriggerEnterEvent;
            Collision.OnTriggerStayEvent += CollisionOnOnTriggerEnterEvent;
            Collision.OnTriggerExitEvent += CollisionOnOnTriggerExitEvent;
        }

        private void CollisionOnOnTriggerEnterEvent(Collider target)
        {
            if (HaveTarget)
            {
                return;
            }
            
            if (target.TryGetComponent<Entity>(out var entity) == false)
            {
                return;
            }

            if (entity.Space != Entity.Space)
            {
                return;
            }

            if (entity.Type == EntityType.Fish)
            {
                if (Entity.PlayerSettings.GetLevel(EPlayerUpgradeType.FishLevel) < ((BaseFish) entity).Level)
                {
                    return;
                }
            }

            if (entity.HaveLife == false)
            {
                return;
            }

            if (Mathf.Abs(transform.position.y - target.transform.position.y) > 3f && entity.Type != EntityType.Fish)
            {
                return;
            }
            
            Target = entity;
        }
        
        private void CollisionOnOnTriggerExitEvent(Collider target)
        {
            if (HaveTarget == false)
            {
                return;
            }
            if (target.TryGetComponent<Entity>(out var entity) == false)
            {
                return;
            }

            if (Target != entity)
            {
                return;
            }

            Target = null;
        }


        private void OnDisable()
        {
            if (_upgradeService != null)
            {
                foreach (var weapon in _weaponData)
                {
                    _upgradeService.RemoveObserver(weapon.Behaviour);
                }
            }
            
            AttackComponent.OnAttackEvent -= DamageTarget;
        }

        private void OnDestroy()
        {
            _observableSelector.RemoveObserver(this);
            Collision.OnTriggerEnterEvent -= CollisionOnOnTriggerEnterEvent;
            Collision.OnTriggerStayEvent -= CollisionOnOnTriggerEnterEvent;
            Collision.OnTriggerExitEvent -= CollisionOnOnTriggerExitEvent;
            if (_stateMachine == null)
            {
                return;
            }
            _stateMachine.OnEnterState -= StateMachineOnOnEnterState;
        }
        
        
        [Button]
        private void HideWeapon()
        {
            Collision.SetRadius(5.5f);
            if (_current == null) return;
            _current.Hide();
            _current = null;
        }
        
        public void PerformNotify(WeaponItem data)
        {
            UseWeapon(data.id);
        }
    }
}
