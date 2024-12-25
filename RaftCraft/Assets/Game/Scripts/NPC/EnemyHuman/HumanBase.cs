using System;
using System.Collections.Generic;
using Game.GameBalanceCore.scripts;
using Game.GameBalanceCore.Scripts.BalanceValue;
using Game.Scripts.BattleMode;
using Game.Scripts.DamageEffector;
using Game.Scripts.DamageEffector.Data;
using Game.Scripts.DamageEffector.Interface;
using Game.Scripts.NPC.Fish;
using Game.Scripts.Player.EntityGame;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.NPC
{
    [RequireComponent(typeof(Rigidbody), typeof(DamageFeedback))]
    public abstract class HumanBase : Entity, IDamagable, ICheckDistance, ICommander
    {
        [Header("Settings")] 
        [SerializeField] protected EnemyProperty _property;
        [SerializeField] private Transform _model;
        [SerializeField] private SkinnedMeshRenderer _skinnedMesh;
        [SerializeField] private PlayerHealthBar _barHeals;

        protected Collider _collider;
        protected Rigidbody _rigidbody;
        protected Animator _animator;
        protected IMovable _moveBehavior;
        protected IAttack _attackBehavior;
        protected NPCNavigation _navigation;
        protected bool _isClimb;
        protected Transform _currentTarget;
        protected IDamagable _damagableTarget;
        protected float _speedMoveModified;

        private ISpawnEffectService _spawnEffectService;
        private DamageFeedback _damageFeedback;
        private DamageEffectSpawner _damageEffectSpawner;
        private HumanState _state;
        private Action _callbackDeath;
        private float _maxHeals;
        private float _cooldown;
        private float _currentHealth;

        public override bool HaveLife => Heals > 0;
        public List<DropItems> Drop => _property.Drop;

        private float Heals
        {
            get => _currentHealth;

            set
            {
                _currentHealth = Mathf.Clamp(value, 0f, _maxHeals);
                if (_barHeals != null)
                {
                    _barHeals.SetValue(_currentHealth / _maxHeals);
                    _barHeals.SetValueTextHeals((int)_currentHealth);
                }
            }
        }

        private readonly int _hashIdleAnim = Animator.StringToHash("IdleAnim");
        private readonly int _hashSpeedRun = Animator.StringToHash("SpeedRun");
        private readonly int _hashDeathAnim = Animator.StringToHash("DeathAnim");
        protected readonly int _hashDeath = Animator.StringToHash("Death");

        public Vector3 Position => transform.position + (Vector3.up * 1.5f);
        public EntityType CurrentType => Type;

        protected HumanState CurrentState
        {
            get => _state;

            set
            {
                if (_state == value) return;

                _state = value;

                switch (value)
                {
                    case HumanState.Idle:
                        EnterIdle();
                        break;
                    case HumanState.Action:
                        EnterAction();
                        break;
                    case HumanState.Attack:
                        EnterAttack();
                        break;
                    case HumanState.Movement:
                        EnterMovement();
                        break;
                    case HumanState.Death:
                        EnterDeath();
                        break;
                }
            }
        }

        public event Action<IDamagable, float> OnDamage;
        public Action<HumanBase, bool> OnDeathCallback;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _damageFeedback = GetComponent<DamageFeedback>();
            _collider = GetComponent<Collider>();
            _animator = _model.GetComponent<Animator>();

            _damageFeedback.SetRenderer(_skinnedMesh);
            _skinnedMesh.materials[_damageFeedback.IndexMaterial].SetColor("_BaseColor", _property.BodyColor); 
        }

        public void Init(NPCNavigation npcNavigation, DamageEffectSpawner damageEffectSpawner, ISpawnEffectService spawnEffectService)
        {
            _damageEffectSpawner = damageEffectSpawner;
            _navigation = npcNavigation;
            _spawnEffectService = spawnEffectService;
            _maxHeals = GameBalance.Instance.GetBalanceValue(_property.Health, TypeValueBalance.PirateHeals);
            Heals = GameBalance.Instance.GetBalanceValue(_property.Health, TypeValueBalance.PirateHeals);
            _currentTarget = _navigation.Player.transform;
            _model.localScale = Vector3.one * Random.Range(_property.ScaleMinMax.x, _property.ScaleMinMax.y);

            var modifierSpeed = Random.Range(0.9f, 1.1f);
            _animator.SetFloat(_hashSpeedRun, modifierSpeed);
            _animator.SetFloat(_hashIdleAnim, Random.Range(0, 2));
            _animator.SetFloat(_hashDeathAnim, Random.Range(0, 2));
            _speedMoveModified = _property.SpeedMove * modifierSpeed;

            OnInit();

            CurrentState = HumanState.Idle;
        }

        public virtual void TakeDamage(float damage, Vector3 target, bool critical = false)
        {
            if (CurrentState == HumanState.Death) return;

            if (!critical)
            {
                damage = Random.Range(damage - 1, damage + 1);
            }

            _moveBehavior.Reset();
            _damageFeedback.TweenExecute();
            _spawnEffectService.SpawnEffect(EffectType.DamageHumanBullet, target);
            _damageEffectSpawner.Spawn(transform.position, damage, critical);

            Heals -= damage;

            OnDamage?.Invoke(this, damage);

            if (Heals <= 0)
            {
                _rigidbody.isKinematic = false;
                _rigidbody.velocity += Vector3.down * 4f;
                _rigidbody.useGravity = true;
                _collider.isTrigger = false;

                CurrentState = HumanState.Death;
            }
        }

        public bool CheckDistance(Vector3 target, float reachedDistance)
        {
            return (transform.position - target).sqrMagnitude < reachedDistance * reachedDistance;
        }

        private new void Update()
        {
            Tick();
            _cooldown += Time.deltaTime;

            if (_cooldown > _property.CooldownUpdatePath)
            {
                _cooldown = 0f;
                var data = _navigation.GetRoute(transform.position);
                
                if (_isClimb && data.isClimb == false)
                {
                    if (Vector3.Distance(transform.position, _currentTarget.position) > 0.3f) return;
                }

                (_currentTarget, _isClimb) = data;
            }
        }

        protected abstract void OnInit();
        protected abstract void Tick();

        protected virtual void EnterMovement()
        {
        }

        protected virtual void EnterAction()
        {
        }

        protected virtual void EnterAttack()
        {
        }

        protected virtual void EnterIdle()
        {
        }

        protected virtual void EnterDeath()
        {
        }

        public virtual void Command(ICommand command)
        {
        }
    }
}