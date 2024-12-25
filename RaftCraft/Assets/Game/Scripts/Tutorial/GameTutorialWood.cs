using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.Scripts.BattleMode;
using Game.Scripts.CollectingResources;
using Game.Scripts.Days;
using Game.Scripts.Extension;
using Game.Scripts.GameIndicator;
using Game.Scripts.Player;
using Game.Scripts.Player.Spawners;
using Game.Scripts.Quest.Interfaces;
using Game.Scripts.Saves;
using Game.Scripts.Tutorial.Old;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Scripts.Tutorial
{
    public class GameTutorialWood : MonoBehaviour, IReadyClient
    {
        [SerializeField] private Transform collectWoodArrow;
        [SerializeField] private Transform extraWoodArrow;
        [SerializeField] private LineRendererMove _prefabLine;
        
        private int _currentCount;
        private int _targetWoodCount;

        private List<BubbleCollectable> _collectables = new();
        private IBattleService _battleService;
        private IDayService _dayService;
        private IPlayerService _playerService;
        
        private bool _isReady;
        private Sprite _woodIcon;
        private LineRendererMove _currentLine;
        private BubbleCollectable _current;
        private Transform _player;
        private GameSave _gameSave;
        private IControllerIndicator _controllerIndicator;

        public event Action<IReadyClient> OnChangeReady;
        public bool IsReady => _isReady;
        public float Delay => 0f;

        [Inject]
        public void Initialize(IDayService dayService, IBattleService battleService, IPlayerService playerService, IControllerIndicator controllerIndicator, GameSave gameSave)
        {
            _gameSave = gameSave;
            _dayService = dayService;
            _battleService = battleService;
            _playerService = playerService;
            _controllerIndicator = controllerIndicator;
            
            var iconsConfig = Resources.Load<IconConfig>("IconConfig");
            _woodIcon = iconsConfig.WoodIcon;
        }
    
        private void Start()
        {
            _isReady = true;
            OnChangeReady?.Invoke(this);
            _battleService.OnSpawnerItem += BattleServiceOnOnChangeState;
            BattleServiceOnOnChangeState();
            _playerService.AddListener(OnChangePlayer);
        }

        private void OnChangePlayer(EPlayerStates state, EntityPlayer player)
        {
            if (state == EPlayerStates.SpawnPlayer)
            {
                _player = player.transform;
            }
        }

        private void BattleServiceOnOnChangeState()
        {
            _currentCount = 0;
            OnChangeReady?.Invoke(this);
            StartCoroutine(WaitSpawnWood());
        }

        private IEnumerator WaitSpawnWood()
        {
            yield return new WaitForSeconds(0.25f);
            ShowCollectWoodArrow();
        }


        private void ShowCollectWoodArrow()
        {
            if (_collectables.Any())
            {
                return;
            }
            
            if (_gameSave.GetData(SaveConstants.TutorialOne, false) == false)
            {
                return;
            }
            _collectables = FindObjectsOfType<BubbleCollectable>().ToList();
            _targetWoodCount = _collectables.Count;
            _isReady = _targetWoodCount == 0;
            OnChangeReady?.Invoke(this);
            if (_targetWoodCount == 0)
            {
                return;
            }

            if (_dayService.CurrentDay < 2)
            {
                if (_currentLine == null)
                {
                    _currentLine = Instantiate(_prefabLine, transform);
                    _currentLine.CountPoint = 2;
                }
                
                if (_current == null)
                {
                    _current = _collectables.GetRandomItem();
                }
            }
            else
            {
                _isReady = true;
                OnChangeReady?.Invoke(this);
                _current = null;
            }
        
            foreach (var bubbleCollectable in _collectables)
            {
                bubbleCollectable.OnDestroy += bubbleCollectable =>
                {
                    _currentCount += 1;
                    _collectables.Remove(bubbleCollectable);
                    if (_current == bubbleCollectable && _collectables.Count > 0)
                    {
                        _current = _collectables.GetRandomItem();
                    }
                    if (_currentCount >= _targetWoodCount)
                    {
                        _current = null;
                        collectWoodArrow.gameObject.SetActive(false);
                        _controllerIndicator.RemoveTarget(collectWoodArrow);
                        _isReady = true;
                        OnChangeReady?.Invoke(this);
                    }
                };
            }

            if (_collectables.Exists(bubble => bubble.transform.position.x < 0) && _collectables.Exists(bubble => bubble.transform.position.x > 0))
            {
                var firstCollection = _collectables.Where(bubble => bubble.transform.position.x < 0);
                var secondCollection = _collectables.Where(bubble => bubble.transform.position.x > 0);
                
                SetIndicator(collectWoodArrow, firstCollection);
                SetIndicator(extraWoodArrow, secondCollection);
                
                return;
            }
            
            SetIndicator(collectWoodArrow, _collectables);
        }

        private void SetIndicator(Transform indicator, IEnumerable<BubbleCollectable> collection)
        {
            var position = indicator.position;
            var bubbleCollectables = collection.ToList();
            position.x = GetAverage(bubbleCollectables);
        
            indicator.position = position;
            var condition = _targetWoodCount > 0;
            indicator.gameObject.SetActive(condition);

            if (condition)
            {
                _controllerIndicator.AddTarget(indicator ,new Vector3(800f, 400f, 0f), 10, _woodIcon, new Color(1,1,1), multiplierOffset: 2f);
            }
            else
            {
                _controllerIndicator.RemoveTarget(indicator);
            }
            
            foreach (var bubbleCollectable in bubbleCollectables)
            {
                bubbleCollectable.OnDestroy += bubble =>
                {
                    bubbleCollectables.Remove(bubble);
                    if (!bubbleCollectables.Any())
                    {
                        indicator.gameObject.SetActive(false);
                        _controllerIndicator.RemoveTarget(indicator);
                    }
                };
            }
        }

        private float GetAverage(IEnumerable<BubbleCollectable> collection)
        {
            var ordered = collection.OrderBy(bubble => bubble.transform.position.x).Select(x => x.transform.position.x).ToArray();
            var xMin = ordered[0];
            var xMax = ordered[^1];

            var avgPos = (xMin + xMax) / 2;
            return avgPos;
        }

        private void Update()
        {
            if (_currentLine == null)
            {
                return;
            }

            if (_current == null || _player == null)
            {
                _currentLine.SetStateLine(false);
                return;
            }
            
            _currentLine.SetStateLine(true);
            _currentLine.SetPosition(0, _player.position);
            _currentLine.SetPosition(1, _current.transform.position);
        }

        private void OnDestroy()
        {
            _playerService.RemoveListener(OnChangePlayer);
            _battleService.OnSpawnerItem -= BattleServiceOnOnChangeState;
        }
    }
}