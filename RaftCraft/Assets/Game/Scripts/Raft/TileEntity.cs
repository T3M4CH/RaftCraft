using System;
using Game.Scripts.BattleMode;
using Game.Scripts.InteractiveObjects;
using Game.Scripts.InteractiveObjects.Interfaces;
using Game.Scripts.NPC;
using Game.Scripts.Player.EntityGame;
using Game.Scripts.Raft.BuildSystem;
using Game.Scripts.Raft.Components;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Game.Scripts.Raft
{
    public class TileEntity : Entity, IDamagable, IInteraction
    {
        public enum TileState
        {
            Life,
            Dead
        }

        [SerializeField] private BuildState _lifeState;
        
        [SerializeField, FoldoutGroup("Parent Tile")]
        private TileBuild _tileBuild;
        
        [SerializeField, FoldoutGroup("Events")]
        private UnityEvent<Vector3> OnDead;

        [SerializeField, FoldoutGroup("Events")]
        private UnityEvent OnHeal;

        [field: SerializeField, FoldoutGroup("Components")]
        public TileBar Bar { get; private set; }


        [SerializeField, FoldoutGroup("Settings")]
        private float _healsTile;

        public Vector3 Position => transform.position;
        public event Action<IDamagable, float> OnDamage;
        public EntityType CurrentType => Type;

        [Button]
        public void TakeDamage(float damage, Vector3 target, bool critical = false)
        {
            Heals -= damage;
            OnDamage?.Invoke(this, damage);
        }

        private float _maxHeals;
        private float _heals;
        private BoxCollider _collider;

        private float Heals
        {
            get => _heals;

            set
            {
                _heals = Mathf.Clamp(value, 0f, _maxHeals);
                if (_heals <= 0f)
                {
                    Dead();
                }
            }
        }

        private TileState _state;

        public bool HaveInteraction => _heals <= 0f;

        [ShowInInspector, ReadOnly]
        public TileState State
        {
            get
            {
                if (_tileBuild != null)
                {
                    if (_tileBuild.GlobalState < _lifeState)
                    {
                        return TileState.Dead;
                    }
                }

                return _state;
            }

            set
            {
                _state = value;
                switch (_state)
                {
                    case TileState.Life:
                        Bar.Hide();
                        _collider.enabled = true;
                        break;
                    case TileState.Dead:
                        _collider.enabled = false;
                        Bar.Show();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private IBattleService _battleService;
        
        private void Awake()
        {
            _collider = GetComponent<BoxCollider>();
            _maxHeals = _healsTile;
            //TODO Получать данные из сохранений
            Heals = _healsTile;
            State = TileState.Life;
        }

        [Inject]
        private void Construct(IBattleService battleService)
        {
            _battleService = battleService;
        }


        private void Start()
        {
            _battleService.OnChangeResult += BattleServiceOnOnChangeState;
        }

        private void BattleServiceOnOnChangeState(bool result)
        {
            if (result == false)
            {
                Heal();
            }
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            _battleService.OnChangeResult -= BattleServiceOnOnChangeState;
        }


        private void Dead()
        {
            State = TileState.Dead;
            OnDead?.Invoke(Vector3.left);
        }

        [Button]
        public void Heal()
        {
            if (Heals > 0f)
            {
                return;
            }
            State = TileState.Life;
            Heals = _maxHeals;
            OnHeal?.Invoke();
        }

        public override bool HaveLife => Heals > 0;
        public bool IsAbleEverywhere => false;
        public bool Interaction => State == TileState.Dead;
        public InteractionType CurrentTypeInteraction => InteractionType.Repair;
        public float DelayAction => 0.1f;


        
        public void Action()
        {
            
        }

        public void EnterInteraction()
        {
            
        }

        public void ExitInteraction()
        {
            
        }
    }
}
