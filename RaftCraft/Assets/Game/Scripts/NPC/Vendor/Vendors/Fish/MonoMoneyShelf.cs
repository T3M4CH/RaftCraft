using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Prefabs.NPC.Vendors;
using Game.Scripts.InteractiveObjects;
using Game.Scripts.InteractiveObjects.Interfaces;
using Game.Scripts.Player;
using Game.Scripts.Player.Spawners;
using Game.Scripts.Pool;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ResourceController.Interfaces;
using Game.Scripts.Saves;
using Sirenix.OdinInspector;
using Reflex.Attributes;
using UnityEngine;

public class MonoMoneyShelf : MonoBehaviour, IInteraction
{
    public event Action<bool> OnTakeMoney;

    [SerializeField] private int maxFloor;
    [SerializeField] private int cellCount;
    [SerializeField] private float delay;
    [SerializeField] private float yOffset;
    [SerializeField] private float delayTime = 2;
    [SerializeField] private float multiplier;
    [SerializeField] private MonoMoney moneyPrefab;
    [SerializeField] private Transform[] moneyPlaces;
    [field: SerializeField] public EResourceType ResourceType { get; private set; }

    private int _currentCount;
    private int _moneyMaxCount;
    private float _currentDelay;
    private Transform _player;
    private GameSave _gameSave;
    private Transform _transform;
    private IResourceService _resourceService;
    private List<MonoMoney> moneyList = new();
    private PoolObjects<MonoMoney> moneyPool;
    private IPlayerService _playerService;

    [Inject]
    private void Construct(GameSave gameSave, IResourceService resourceService, IPlayerService playerService)
    {
        _gameSave = gameSave;
        _resourceService = resourceService;
        _playerService = playerService;
        _playerService.AddListener(ValidatePlayer);
    }

    public void Action()
    {
        _currentDelay *= multiplier;

        if (!TakeMoney())
        {
            ExitInteraction();
        }
    }

    public void EnterInteraction()
    {
        _currentDelay = delay;
    }

    public void ExitInteraction()
    {
        _currentDelay = delay;
    }

    private bool TakeMoney()
    {
        if (_currentCount > 0)
        {
            var money = moneyList[^1];
            money.gameObject.SetActive(false);
            var moneyCost = _currentCount / moneyList.Count;
            _currentCount -= moneyCost;
            moneyList.Remove(money);
            _resourceService.Add(ResourceType, moneyCost);

            _gameSave.SetData(ResourceType == EResourceType.CoinBlue ? SaveConstants.DollarsInShelf : SaveConstants.CoinsOnShelf, _currentCount);
            OnTakeMoney?.Invoke(true);
            return true;
        }

        OnTakeMoney?.Invoke(false);
        return false;
    }

    [Button]
    public void AddMoney(int count, bool save = true)
    {
        var timeSegment = delayTime / count;
        for (int i = 0; i < count; i++)
        {
            var moneyCount = moneyList.Count;
            var id = moneyCount % cellCount;
            var floor = moneyCount / cellCount;

            if (ResourceType == EResourceType.CoinGold && moneyCount < _moneyMaxCount)
            {
                var place = moneyPlaces[id];
                var moneyInstance = moneyPool.GetFree(true);
                var money = moneyInstance.transform;
                money.position = place.position;
                money.rotation = place.rotation;
                var position = money.position;
                position.y += floor * yOffset;
                money.position = position;

                if (save)
                {
                    money.DOScale(1, timeSegment).SetEase(Ease.OutCirc).From(0).SetLink(moneyInstance.gameObject);
                }

                money.SetParent(_transform);

                moneyList.Add(moneyInstance);
            }
            
            _currentCount += 1;
        }
        
        if (save)
        {
            _gameSave.SetData(ResourceType == EResourceType.CoinBlue ? SaveConstants.DollarsInShelf : SaveConstants.CoinsOnShelf, _currentCount);
        }
    }

    private void ValidatePlayer(EPlayerStates state, EntityPlayer player)
    {
        _player = player.Hips;
    }

    private void Start()
    {
        _transform = transform;

        moneyPool = new PoolObjects<MonoMoney>(moneyPrefab, _transform, 30);

        _moneyMaxCount = cellCount * maxFloor;

        var targetCount = 0;
        if (ResourceType == EResourceType.CoinBlue)
        {
            targetCount = _gameSave.GetData(SaveConstants.DollarsInShelf, 0);
        }
        else
        {
            targetCount = _gameSave.GetData(SaveConstants.CoinsOnShelf, 0);
        }

        AddMoney(targetCount, false);
    }

    public bool Interaction => true;
    public bool IsAbleEverywhere => true;
    public float DelayAction => _currentDelay;
    public InteractionType CurrentTypeInteraction => InteractionType.Build;
}