using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Scripts.BattleMode;
using Game.Scripts.CameraSystem.Cameras;
using Game.Scripts.CameraSystem.Enums;
using Game.Scripts.CameraSystem.Interfaces;
using Game.Scripts.Core.Interface;
using Game.Scripts.DamageEffector;
using Game.Scripts.DamageEffector.Interface;
using Game.Scripts.Days;
using Game.Scripts.Joystick.Interfaces;
using Game.Scripts.NPC;
using Game.Scripts.Player.EntityGame;
using Game.Scripts.Player.HeroPumping;
using Game.Scripts.Player.HeroPumping.Interfaces;
using Game.Scripts.Player.PlayerController.PlayerStates;
using Game.Scripts.Player.StateMachine;
using Game.Scripts.Player.WeaponController;
using Game.Scripts.Player.WeaponController.WeaponInventory;
using Game.Scripts.Quest.Interfaces;
using Game.Scripts.Raft.Interface;
using Game.Scripts.ResourceController.LocalPlayerResources;
using Game.Scripts.Saves;
using Game.Scripts.UI.WindowManager;
using Game.Scripts.UI.WindowManager.Windows;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.Player.Spawners
{
    public class PlayerController : MonoBehaviour, IPlayerService, IReadyClient
    {
        private event Action<EPlayerStates ,EntityPlayer> OnUpdateStatePlayer;

        [SerializeField, FoldoutGroup("Prefab Player")] private EntityPlayer _prefabPlayer;
        [SerializeField] private PlayerUpgradesConfig _playerUpgradesConfig;
        [SerializeField] private SerializablePlayerSpawnerSettings _spawnerSettings;
        
        private ICameraService _cameraService;
        private IJoystickService _joystickService;
        private DamageEffectSpawner _effectSpawner;
        private PlayerUpgradeController _upgradeController;
        private IBattleService _battleService;
        private IRaftStructures _raftStructures;
        private WindowBattleFail _windowBattleFail;
        private ISpawnEffectService _spawnEffectService;
        private IDayService _dayService;
        private IInventoryWeapon _inventoryWeapon;
        private IGameObservable<List<WeaponItem>> _observableWeapon;
        private IGameObservable<WeaponItem> _observableSelectorWeapon;
        private IGameObservable<WeaponUpgrade> _upgradeService;
        
        private static readonly int IsWater = Animator.StringToHash("IsWater");
        private static readonly int IsGround = Animator.StringToHash("IsGround");
        private static readonly int SpawnedInWater = Animator.StringToHash("SpawnedInWater");

        private EntityPlayer Player { get; set; }
        public PlayerResources PlayerResources { get; private set; }
        public IPlayerUpgradeSettings UpgradeSettings { get; private set; }
        public IPlayerUpgradeService UpgradeService { get; private set;}
        private bool _ready;
        public event Action<IReadyClient> OnChangeReady;
        public bool IsReady => _ready;
        public float Delay => 0f;

        private void Awake()
        {
            _upgradeController = new PlayerUpgradeController(_playerUpgradesConfig);
            PlayerResources = new PlayerResources(this, _upgradeController);
            UpgradeSettings = _upgradeController;
            UpgradeService = _upgradeController;
        }

        [Inject]
        private void Init(ICameraService cameraService, WindowManager windowManager, DamageEffectSpawner effectSpawner, 
            IBattleService battleService, IRaftStructures raftStructures, GameSave gameSave, IDayService dayService,
            ISpawnEffectService spawnEffectService, IGameObservable<WeaponUpgrade> upgradeService,
            IInventoryWeapon inventoryWeapon, IGameObservable<List<WeaponItem>> observableWeapon, IGameObservable<WeaponItem> observableSelectorWeapon)
        {
            _observableSelectorWeapon = observableSelectorWeapon;
            _inventoryWeapon = inventoryWeapon;
            _observableWeapon = observableWeapon;
            _upgradeService = upgradeService;
            _dayService = dayService;
            _spawnEffectService = spawnEffectService;
            _raftStructures = raftStructures;
            _battleService = battleService;
            _effectSpawner = effectSpawner;
            _cameraService = cameraService;
            _upgradeController.Load(gameSave);

            var windowGame = windowManager.GetWindow<WindowGame>();
            _windowBattleFail = windowManager.GetWindow<WindowBattleFail>();
            try
            {
                windowManager.OpenWindow<WindowDayProgress>();
            }
            catch (Exception e)
            {
                // ignored
            }

            windowGame.Show();
            _joystickService = windowGame.Joystick;

            _dayService.OnDayStart += ValidateDay;
            _battleService.OnChangeState += OnChangeBattleState;
            _windowBattleFail.OnClickContinue += SpawnPlayer;
        }

        private void ValidateDay(int obj)
        {
            if (Player != null)
            {
                Player.UpdateStats();
            }
        }

        private void Start()
        {
            SpawnPlayer();
        }

        [Button]
        private void TextDamage(int damage)
        {
            Player.TakeDamage(damage, Vector3.zero);
        }

        [Button]
        public void SpawnPlayer()
        {
            var player = Instantiate(_prefabPlayer);
            if (Player != null)
            {
                Player.StateMachine.OnEnterState -= StateMachineOnOnEnterState;
                Player.OnDamage -= PlayerOnOnDamage;
            }

            Player = player;
            Player.Initialize(_upgradeController, _joystickService, _dayService);
            Player.Weapon.InitSystem(_joystickService, _upgradeService, _inventoryWeapon, _observableSelectorWeapon);
            player.transform.position = _spawnerSettings.SpawnPosition;
            player.Rb.position = _spawnerSettings.SpawnPosition;
            var stateMachine = new EntityStateMachine();
            var plotState = new PlayerPlotState(stateMachine, player, _joystickService, _battleService, _raftStructures);
            var jumpToWaterState = new PlayerJumpToWater(stateMachine, player);
            var waterState = new PlayerWaterState(stateMachine, player, _joystickService);
            var jumpToRaft = new PlayerJumpToRaft(stateMachine, player);
            var deathInWater = new PlayerDeathInWater(Player.Animator, Player.MainCollider, Player.Fragments, _spawnEffectService, stateMachine, Player);
            var deathInGround = new PlayerDeathInGround(Player.Animator, Player.MainCollider, Player.Fragments,stateMachine, Player);
            var ladderState = new PlayerLadderMove(stateMachine, Player, _joystickService);
            var fallState = new PlayerFallState(stateMachine, Player);
            var stateFallWater = new PlayerFallWater(stateMachine, player);
            
            stateMachine.AddState(jumpToRaft);
            stateMachine.AddState(plotState);
            stateMachine.AddState(jumpToWaterState);
            stateMachine.AddState(waterState);
            stateMachine.AddState(deathInWater);
            stateMachine.AddState(deathInGround);
            stateMachine.AddState(ladderState);
            stateMachine.AddState(fallState);
            stateMachine.AddState(stateFallWater);
            player.InitState(stateMachine);

            player.StateMachine.OnEnterState += StateMachineOnOnEnterState;
            player.OnDamage += PlayerOnOnDamage;
            player.PlayerController = this;

            _cameraService.SetLookAndFollowToAll(player.transform);

            switch (_spawnerSettings.CameraType)
            {
                case ECameraType.PlayerCamera:
                    _cameraService.ChangeActiveCamera<PlayerVirtualCamera>().ResetDamping(Vector3.zero, Vector2.zero, 2).Forget();
                    break;
                case ECameraType.SideCamera:
                    _cameraService.ChangeActiveCamera<SideVirtualCamera>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _cameraService.SetFovInstantly(30);
            _cameraService.SetFov(60, 0.5f);
            
            stateMachine.SetState<PlayerPlotState>();
            OnUpdateStatePlayer?.Invoke(EPlayerStates.SpawnPlayer,player);
            OnUpdateStatePlayer?.Invoke(EPlayerStates.PlayerInRaft,player);
        }

        private void PlayerOnOnDamage(IDamagable target, float damage)
        {
            if (Player != null)
            {
                _effectSpawner.Spawn(Player.transform.position, damage, type: EntityType.Player);
            }
        }

        private void OnChangeBattleState(BattleState state)
        {
            if(state == BattleState.Idle)
            {
                if(Player != null)
                    Player.Heal();
            }
        }
        
        private void Update()
        {
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                Debug.Break();
            }
        }

        private void StateMachineOnOnEnterState(EntityState state, Entity entity)
        {
            switch (state)
            {
                case PlayerPlotState:
                    _ready = true;
                    OnChangeReady?.Invoke(this);
                    OnUpdateStatePlayer?.Invoke(EPlayerStates.PlayerInRaft, Player);
                    if (_battleService.CurrentState == BattleState.Idle)
                    {
                        Player.Heal();
                    }
                    _joystickService.SetStateDefault();
                    break;
                case PlayerJumpToWater:
                    _ready = false;
                    OnChangeReady?.Invoke(this);
                    OnUpdateStatePlayer?.Invoke(EPlayerStates.PlayerInWater, Player);
                    break;
                case PlayerWaterState:
                    _ready = false;
                    OnChangeReady?.Invoke(this);
                    OnUpdateStatePlayer?.Invoke(EPlayerStates.PlayerInWater, Player);
                    _joystickService.SetStateWater();
                    break;
                case PlayerDeathInGround:
                    _ready = false;
                    OnChangeReady?.Invoke(this);
                    OnUpdateStatePlayer?.Invoke(EPlayerStates.PlayerDead, Player);
                    OnUpdateStatePlayer?.Invoke(EPlayerStates.PlayerDeadInRaft, Player);
                    Player.StateMachine.OnEnterState -= StateMachineOnOnEnterState;
                    break;
                case PlayerDeathInWater:
                    _ready = false;
                    OnChangeReady?.Invoke(this);
                    OnUpdateStatePlayer?.Invoke(EPlayerStates.PlayerDead, Player);
                    OnUpdateStatePlayer?.Invoke(EPlayerStates.PlayerDeadInWater, Player);
                    Player.StateMachine.OnEnterState -= StateMachineOnOnEnterState;
                    WaitRespawnPlayer();
                    break;
                case PlayerFallWater:
                    _ready = false;
                    OnChangeReady?.Invoke(this);
                    break;
            }
        }

        private async void WaitRespawnPlayer()
        {
            Player = null;
            await RespawnPlayer();
        }

        private async UniTask RespawnPlayer()
        {
            await UniTask.Delay(2000);
            SpawnPlayer();
        }

        private void OnDestroy()
        {
            _windowBattleFail.OnClickContinue -= SpawnPlayer;
            if (Player != null)
            {
                Player.OnDamage -= PlayerOnOnDamage;
            }

            if (_dayService != null)
            {
                _dayService.OnDayStart -= ValidateDay;
            }

            _battleService.OnChangeState -= OnChangeBattleState;
        }

        public void AddListener(Action<EPlayerStates, EntityPlayer> actionSpawn)
        {
            OnUpdateStatePlayer += actionSpawn;
            if (Player != null)
            {
                actionSpawn?.Invoke(EPlayerStates.SpawnPlayer,Player);
            }
        }

        public void RemoveListener(Action<EPlayerStates, EntityPlayer> actionSpawn)
        {
            OnUpdateStatePlayer -= actionSpawn;
        }
    }
}