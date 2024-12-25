using System;
using System.Collections;
using System.Collections.Generic;
using Game.Scripts.BattleMode;
using Game.Scripts.Core.Interface;
using Game.Scripts.Player;
using Game.Scripts.Player.Spawners;
using Game.Scripts.Quest.Interfaces;
using Game.Scripts.ResourceController;
using Game.Scripts.ResourceController.Interfaces;
using Game.Scripts.Saves;
using Game.Scripts.Tutorial.Old;
using Game.Scripts.UI.WindowManager;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Scripts.Tutorial.TutorialBuildVendor
{
    public class TutorialBuildController : MonoBehaviour, IGameObserver<ResourceItem>, IReadyClient
    {
        public event Action<IReadyClient> OnChangeReady;
        public bool IsReady => Current == null;
        public float Delay => 0f;

        [SerializeField] private List<TutorialBuildVendor> _tutorialBuild = new List<TutorialBuildVendor>();
        [SerializeField] private LineRendererMove _prefabLine;
        
        private IPlayerService _playerService;
        private IGameResourceService _gameResourceService;
        private IGameObservable<ResourceItem> _observable;
        private GameSave _gameSave;

        [SerializeField]
        private TutorialBuildVendor _current;

        private TutorialBuildVendor Current
        {
            get => _current;
            set
            {
                if (value == null && _current != null)
                {
                    _current.OnComplete -= OnComplete;
                }
                
                _current = value;
                if (_current != null)
                {
                    _current.OnComplete += OnComplete;
                    Debug.Log($"Selected Tutorial Build: {_current.gameObject}");
                }
                OnChangeReady?.Invoke(this);
            }
        }

        private LineRendererMove _lineRenderer;
        private Transform _player;
        private IBattleService _battleService;
        
        private void OnComplete(TutorialBuildVendor tutorial)
        {
            tutorial.Complete(_gameSave);
            Current = null;
        }

        [Inject]
        private void Construct(GameSave gameSave, IPlayerService playerService, IGameResourceService resourceService, IGameObservable<ResourceItem> observable, IBattleService battleService)
        {
            _gameSave = gameSave;
            _observable = observable;
            _playerService = playerService;
            _gameResourceService = resourceService;
            _battleService = battleService;
        }


        private void Start()
        {
            StartCoroutine(WaitStart());
        }

        private IEnumerator WaitStart()
        {
            yield return new WaitForSeconds(0.1f);
            StartSystem();
        }

        private void BattleServiceOnOnChangeState(BattleState state)
        {
            if (state == BattleState.Idle)
            {
                if (Current == null)
                {
                    Current = GetTutorial();
                    if (Current != null)
                    {
                        Current.StartTutorial();
                    }
                }
            }
        }

        private void OnChangePlayer(EPlayerStates state, EntityPlayer player)
        {
            if (state == EPlayerStates.SpawnPlayer)
            {
                _player = player.Hips;
            }
        }

        private void Update()
        {
            if (_lineRenderer == null)
            {
                return;
            }
            if (Current == null || Current.Target() == null || _player == null)
            {
                _lineRenderer.SetStateLine(false);
                return;
            } 
            
            
            _lineRenderer.SetStateLine(Current.ShowArrow());
            _lineRenderer.SetPosition(0, _player.position);
            _lineRenderer.SetPosition(1, Current.Target().position);
        }

        private void StartSystem()
        {
            _observable.AddObserver(this);
            _lineRenderer = Instantiate(_prefabLine, transform);
            _lineRenderer.CountPoint = 2;
            _lineRenderer.SetStateLine(false);
            _playerService.AddListener(OnChangePlayer);
            _battleService.OnChangeState += BattleServiceOnOnChangeState;
        }

        public void PerformNotify(ResourceItem data)
        {
            if (_battleService.CurrentState != BattleState.Idle)
            {
                return;
            }
            if (Current == null)
            {
                Current = GetTutorial();
                if (Current != null)
                {
                    Current.StartTutorial();
                }
            }
        }

        private TutorialBuildVendor GetTutorial()
        {
            foreach (var build in _tutorialBuild)
            {
                if (build.HaveComplete(_gameSave) == false && build.HaveResource(_gameResourceService))
                {
                    return build;
                }
            }

            return null;
        }

        private void OnDestroy()
        {
            _observable.RemoveObserver(this);
            _playerService.RemoveListener(OnChangePlayer);
            _battleService.OnChangeState -= BattleServiceOnOnChangeState;
        }
    }
}
