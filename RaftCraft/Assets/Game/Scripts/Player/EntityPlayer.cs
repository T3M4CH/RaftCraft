using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.Core;
using Game.Scripts.Days;
using Game.Scripts.Joystick.Interfaces;
using Game.Scripts.NPC;
using Game.Scripts.NPC.Fish.Systems;
using Game.Scripts.NPC.Interface;
using Game.Scripts.Player.Collision;
using Game.Scripts.Player.EntityGame;
using Game.Scripts.Player.HeroPumping.Enums;
using Game.Scripts.Player.HeroPumping.Interfaces;
using Game.Scripts.Player.Oxygen;
using Game.Scripts.Player.PlayerController;
using Game.Scripts.Player.PlayerController.PlayerStates;
using Game.Scripts.Player.RigginController;
using Game.Scripts.ResourceController.LocalPlayerResources;
using GTapSoundManager.SoundManager;
using Lofelt.NiceVibrations;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Player
{
    public class EntityPlayer : Entity, IDamagable, ISpace
    {
        [SerializeField] private SoundAsset _damageSoundAsset;
        [SerializeField, FoldoutGroup("View")] private Animator _animator;
        [SerializeField] private float regenerationTick = 0.2f;

        public event Action<IDamagable, float> OnDamage;

        private float _heals;
        private float _maxHeals;
        private CancellationTokenSource _tokenSource;

        public void Initialize(IPlayerUpgradeSettings playerUpgradeSettings, IJoystickService joystickService, IDayService dayService)
        {
            DayService = dayService;
            JoystickService = joystickService;
            PlayerSettings = playerUpgradeSettings;
            PlayerSettings.OnUpgrade += ValidateUpgrade;
            ValidateUpgrade(EPlayerUpgradeType.MaxHealth);
        }

        public void UpdateStats()
        {
            ValidateUpgrade(EPlayerUpgradeType.MaxHealth);
        }

        private void ValidateUpgrade(EPlayerUpgradeType upgradeType)
        {
            _maxHeals = PlayerSettings.GetValue<float>(EPlayerUpgradeType.MaxHealth);
            Heals = _maxHeals;
        }

        public override LocationSpace Space
        {
            get
            {
                if (_stateMachine == null)
                {
                    return LocationSpace.Ground;
                }

                switch (_stateMachine.CurrentEntityState)
                {
                    case PlayerPlotState:
                        return LocationSpace.Ground;
                    case PlayerWaterState:
                        return LocationSpace.Water;
                }

                return LocationSpace.Water;
            }
        }

        public void TakeDamage(float damage, Vector3 pointDamage = default, bool critical = false)
        {
            if (HaveLife == false)
            {
                return;
            }

            OnDamage?.Invoke(this, damage);
            _damageSoundAsset.Play(Random.Range(0.95f, 1.05f));
            HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
            Heals -= damage;
        }

        public void Heal()
        {
            if (_tokenSource != null)
            {
                _tokenSource.Cancel();
                _tokenSource.Dispose();
            }

            _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;

            UniTask.Void(async () =>
            {
                while (Heals < _maxHeals)
                {
                    await UniTask.WaitForSeconds(regenerationTick, cancellationToken: token).SuppressCancellationThrow();

                    Heals += 1;

                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                }
            });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_tokenSource != null)
            {
                _tokenSource.Cancel();
                _tokenSource.Dispose();
            }

            PlayerSettings.OnUpgrade -= ValidateUpgrade;
        }

        private float Heals
        {
            get => _heals;

            set
            {
                _heals = Mathf.Clamp(value, 0f, _maxHeals);
                _healthBar.SetValue(_heals / _maxHeals);
                _healthBar.SetValueTextHeals((int)_heals);
                if (_heals <= 0)
                {
                    if (StateMachine.CurrentEntityState.GetType() == typeof(PlayerWaterState))
                    {
                        StateMachine.SetState<PlayerDeathInWater>();
                    }
                    else
                    {
                        StateMachine.SetState<PlayerDeathInGround>();
                    }
                }
            }
        }

        public IDayService DayService { get; private set; }
        public EntityType CurrentType => Type;

        public IJoystickService JoystickService { get; private set; }
        public IPlayerUpgradeSettings PlayerSettings { get; private set; }
        public Spawners.PlayerController PlayerController { get; set; }
        [field: SerializeField] public EntityPlayerSettings Settings { get; private set; }
        [field: SerializeField] public Collider MainCollider { get; private set; }
        [field: SerializeField] public MonoRagdollFragment[] Fragments { get; private set; }
        [field: SerializeField] public Rigidbody Rb { get; private set; }
        [field: SerializeField] public EntityCollisionRayCast RayCast { get; private set; }
        [field: SerializeField] public Transform Hips { get; private set; }
        [field: SerializeField] public Transform Character { get; private set; }
        [field: SerializeField] public WeaponController.WeaponController Weapon { get; private set; }
        [field: SerializeField] public PlayerResourceView PlayerResources { get; private set; }
        [field: SerializeField] public GameObject EffectBubbleWater { get; private set; }
        [field: SerializeField, AssetsOnly] public ParticleController EffectSplashWater { get; private set; }
        [field: SerializeField] public FishingSystem FishingSystem { get; private set; }

        [field: SerializeField] public PlayerHealthBar _healthBar;
        [field: SerializeField] public OxygenController OxygenController { get; private set; }
        public Animator Animator => _animator;

        public Vector3 Position => transform.position;
        public override bool HaveLife => Heals > 0f;
    }
}