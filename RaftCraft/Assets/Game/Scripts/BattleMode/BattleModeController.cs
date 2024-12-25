using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.CameraSystem.Interfaces;
using Game.Scripts.DamageEffector;
using Game.Scripts.DamageEffector.Interface;
using Game.Scripts.Days;
using Game.Scripts.Extension;
using Game.Scripts.NPC;
using Game.Scripts.Player.Spawners;
using Game.Scripts.Quest.Interfaces;
using Game.Scripts.Raft.Interface;
using Game.Scripts.ResourceController.Interfaces;
using Game.Scripts.Sound;
using Game.Scripts.Sound.Interfaces;
using Game.Scripts.TimeController;
using Game.Scripts.UI.WindowManager;
using Game.Scripts.UI.WindowManager.Windows;
using GTapSoundManager.SoundManager;
using Reflex.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.BattleMode
{
    public class BattleModeController : MonoBehaviour, IBattleService, IReadyClient
    {
        [SerializeField] private GameObject confettiBlasts;
        [SerializeField] private SoundAsset _winSound;
        [SerializeField] private SoundAsset _loseSound;
        [SerializeField] private EnemySets _enemySets;
        [SerializeField] private float _offsetSpawnEnemy = 40f;
        [SerializeField] private float _durationCutscene = 3f;
        [SerializeField] private float _delayAfterWin = 1f;

        private int _currentDay;
        private List<EnemyRaft> _rafts = new List<EnemyRaft>();
        private IResourceSpawner _resourceSpawner;
        private WindowManager _windowManager;
        private IDayService _dayController;
        private IPlayerService _playerService;
        private NPCNavigation _npcNavigation;
        private WindowBattleFail _windowBattleFail;
        private WindowBattle _windowBattle;
        private DamageEffectSpawner _damageEffectSpawner;
        private ISpawnEffectService _spawnEffectService;
        private CutScene _cutScene;
        private IRaftStructures _raftConstructor;
        private IMusicService _musicService;
        private ITimeController _timeController;
        private ICameraService _cameraService;
        private BattleStatistics _battleStatistics;
        
        public event Action<BattleState> OnChangeState;
        public event Action<IReadyClient> OnChangeReady;
        public event Action OnSpawnerItem;
        public event Action<bool> OnChangeResult;

        private bool _isReady;
        public bool IsReady => _isReady;
        public float Delay => 10f;
        
        private BattleState _currentState;

        public BattleState CurrentState
        {
            get => _currentState;
            private set
            {
                _currentState = value;
                switch (_currentState)
                {
                    case BattleState.СutScene:
                        _isReady = false;
                        _timeController.SetNight();
                        break;
                    case BattleState.Fight:
                        _isReady = false;
                        _timeController.SetNight();
                        break;
                    case BattleState.Result:
                        _isReady = false;
                        break;
                    case BattleState.Idle:
                        _timeController.SetDay();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                OnChangeReady?.Invoke(this);
                OnChangeState?.Invoke(value);
            }
        }

        [Inject]
        private void Init(WindowManager windowManager, ICameraService cameraService, IResourceSpawner resourceSpawner, IDayService dayController, IPlayerService playerService,
            DamageEffectSpawner damageEffectSpawner, IRaftStructures raftConstructor, ISpawnEffectService spawnEffectService, IMusicService musicService, ITimeController timeController)
        {
            _timeController = timeController;
            _musicService = musicService;
            _spawnEffectService = spawnEffectService;
            _damageEffectSpawner = damageEffectSpawner;
            _playerService = playerService;
            _dayController = dayController;
            _windowManager = windowManager;
            _resourceSpawner = resourceSpawner;
            _raftConstructor = raftConstructor;
            _cameraService = cameraService;

            _cutScene = new CutScene(_cameraService);
            _npcNavigation = new NPCNavigation(raftConstructor, _playerService);
            _battleStatistics = new BattleStatistics(playerService);

            _windowBattleFail = windowManager.GetWindow<WindowBattleFail>();
            _windowBattle = windowManager.GetWindow<WindowBattle>();

            windowManager.GetWindow<WindowGame>().OnClickButtonBattle += RunCutScene;

            _windowBattle.OnClickEndDay += OnClickWin;
            _windowBattleFail.OnClickContinue += OnClickFail;
            _dayController.OnDayStart += OnChangeDay;
            
            OnChangeDay(_dayController.CurrentDay);
        }

        private void Start()
        {
            _isReady = true;
            CurrentState = BattleState.Idle;
        }

        private void RunCutScene()
        {
            CurrentState = BattleState.СutScene;

            _windowManager.OpenWindow<WindowBattle>();
            _windowManager.CloseWindow<WindowDayProgress>();

            SpawnEnemy();

            if (_currentDay <= 2)
            {
                _cutScene.PlayScene(_rafts.Peek().transform, _durationCutscene, StartBattle, 0f);
            }
            else
            {
                StartBattle();
            }

            _cutScene.WaterChangeColor(true, _durationCutscene);
        }

        private void StartBattle()
        {
            CurrentState = BattleState.Fight;
            _musicService.PlayMusic(EMusicType.Fight, 2);
            _cameraService.SetFov(70, 0.5f);
        }

        private void ResultBattle(bool result)
        {
            _battleStatistics.OnPlayerWin -= ResultBattle;

            CurrentState = BattleState.Result;

            confettiBlasts.SetActive(false);
            _musicService.StopMusic();
            if (result)
            {
                confettiBlasts.SetActive(true);
                _winSound.Play(Random.Range(0.95f, 1.05f));
                
                _windowManager.OpenWindow<WindowBattleComplete>();

                if (_currentDay <= 1)
                    _windowBattle.ShowEndDayButton();
                else
                    OnClickWin();
            }
            else
            {
                _loseSound.Play(Random.Range(0.95f, 1.05f));
                _windowManager.CloseWindow<WindowBattle>();
                _windowManager.OpenWindow<WindowBattleFail>();
            }
            
            OnChangeResult?.Invoke(result);
        }

        private void OnClickFail()
        {
            foreach (var raft in _rafts)
            {
                raft.DestroyRaft(false);
            }
            
            _isReady = true;
            CurrentState = BattleState.Idle;
            _dayController.RestartDay();
            
            _raftConstructor.OffsetHorizontal = Vector2.zero;
            _windowManager.OpenWindow<WindowDayProgress>();
            _cutScene.ResetCutscene();
        }

        private void OnClickWin()
        {
            CurrentState = BattleState.Idle;
            _raftConstructor.OffsetHorizontal = Vector2.zero;
            _windowManager.CloseWindow<WindowBattle>();
            
            foreach (var raft in _rafts)
            {
                raft.DestroyRaft(true, () => OnSpawnerItem?.Invoke());
            }

            if (_currentDay <= 2)
            {
                _cutScene.PlayScene(_rafts.Peek().transform, _durationCutscene, EndBattle, _delayAfterWin);
            }
            else
            {
                EndBattle();
            }

            _cutScene.WaterChangeColor(false, _durationCutscene);
        }

        private void EndBattle()
        {
            _isReady = true;
            OnChangeReady?.Invoke(this);
            
            _dayController.CompleteDay();
            _windowManager.OpenWindow<WindowDayProgress>();
            
            _musicService.StopMusic();
            _musicService.PlayMusic(EMusicType.Peaceful, 15);
            _cameraService.SetFov(60, 0.5f);
        }

        private void OnChangeDay(int dayNumber)
        {
            _currentDay = dayNumber;
        }

        private void SpawnEnemy()
        {
            _rafts.Clear();
            // выбор конфига согласно дню
            var data = _enemySets.EnemySetPerDay.FirstOrDefault(pair => pair.DayId == _currentDay);

            if (data.EnemyRaft == null)
                data = Enumerable.Last(_enemySets.EnemySetPerDay);
            
            var queueFirst = new List<HumanBase>();
            var queueSecond = new List<HumanBase>();
            var counter = 0;

            // распределение общего конфига пиратов на два списка для первого и второго плота, даже если планируется один. 
            // также подсчет общего кол-ва пиратов для контроля прогресса
            foreach (var queue in data.Queue)
            {
                for (int i = 0; i < queue.Amount; i++)
                {
                    if(counter % 2 == 0)
                        queueFirst.Add(queue.Human);
                    else
                        queueSecond.Add(queue.Human);

                    counter++;
                }
            }
            
            // объединение в один список если плот один
            var countRaft = Mathf.Min(data.EnemyRaft.Length, 2);
            if (countRaft == 1)
            {
                queueFirst.AddRange(queueSecond);
            }
            
            // спавн инициализация плотов
            for (int i = 0; i < countRaft; i++)
            {
                var positionSpawn = new Vector3(i == 0 ? -_offsetSpawnEnemy : _offsetSpawnEnemy, 0f, 0f);
                var raft = Instantiate(data.EnemyRaft[i], positionSpawn, Quaternion.identity);
                
                raft.Init(
                    _npcNavigation, 
                    _windowBattle, 
                    _resourceSpawner, i == 0 ? queueFirst.ToArray() : queueSecond.ToArray(), 
                    data.DropItems, 
                    _damageEffectSpawner, 
                    _spawnEffectService, 
                    _battleStatistics);
                
                _rafts.Add(raft);
            }

            _battleStatistics.Setup(counter);
            _battleStatistics.OnPlayerWin += ResultBattle;
            _battleStatistics.OnUpdateProgress += _windowBattle.ChangeValueProgressBar;
        }

        private void OnDestroy()
        {
            _windowManager.GetWindow<WindowGame>().OnClickButtonBattle -= RunCutScene;
            _battleStatistics.OnPlayerWin -= ResultBattle;
            _battleStatistics.OnUpdateProgress -= _windowBattle.ChangeValueProgressBar;
            _windowBattle.OnClickEndDay -= OnClickWin;
            _windowBattleFail.OnClickContinue -= OnClickFail;
            _dayController.OnDayStart -= OnChangeDay;

            _npcNavigation.Destroy();
            _battleStatistics.Destroy();
            _cutScene.ResetCutscene();
        }
    }
}