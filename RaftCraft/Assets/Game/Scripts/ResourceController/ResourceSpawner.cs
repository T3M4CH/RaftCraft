using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.ResourceController.Interfaces;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.CollectingResources;
using Game.Scripts.GameIndicator;
using Game.Scripts.Player.StateMachine;
using Game.Scripts.Player.Spawners;
using Game.Scripts.Player;
using Game.Scripts.Player.HeroPumping.Enums;
using Game.Scripts.Player.HeroPumping.Interfaces;
using Game.Scripts.Pool;
using Game.Scripts.ResourceController;
using Game.Scripts.ResourceController.LocalPlayerResources;
using Game.Scripts.ResourceController.UI;
using Game.Scripts.Saves;
using UnityEngine;
using Object = UnityEngine.Object;

public class ResourceSpawner : IResourceSpawner, IStartableElement, IDisposable
{
    public event Action OnCreated = () => { };

    private EntityStateMachine _currentStateMachine;
    private PoolObjects<BubbleCollectable> _bubblePool;
    private PoolObjects<CollectingResourceObject> _resourcesPool;
    private PoolObjects<CollectingResourceObject> _playerDeadResources;

    private float _maxDepth;
    private CollectingResourceObject _currentLostResource;

    private readonly Sprite _tombIcon;
    private readonly GameSave _gameSave;
    private readonly IPlayerService _playerService;
    private readonly ResourcesPrefabs _bubblePrefab;
    private readonly PlayerResources _playerResources;
    private readonly IResourceService _resourceService;
    private readonly IPlayerUpgradeSettings _playerUpgradeSettings;
    private readonly IControllerIndicator _indicatorControllerController;
    private readonly IGameResourceService _gameResource;
    private const float LossPercentage = 0.3f;

    //TODO: Заменить на сериализуемую структуру
    public ResourceSpawner(GameSave gameSave, IControllerIndicator indicatorController, ResourcesPrefabs bubblePrefab, IResourceService resourceService, IPlayerService playerService, IGameResourceService gameResource)
    {
        _tombIcon = Resources.Load<IconConfig>("IconConfig").TombIcon;

        _gameSave = gameSave;
        _gameResource = gameResource;
        _bubblePrefab = bubblePrefab;
        _resourceService = resourceService;
        _playerResources = playerService.PlayerResources;
        _playerService = playerService;
        _playerUpgradeSettings = _playerService.UpgradeSettings;

        _indicatorControllerController = indicatorController;

        playerService.AddListener(ValidateStates);
    }

    public void Execute()
    {
        _playerDeadResources = new PoolObjects<CollectingResourceObject>(_bubblePrefab.PrefabResource, new GameObject("DeadResources").transform, 2);
        _resourcesPool = new PoolObjects<CollectingResourceObject>(_bubblePrefab.PrefabResource, new GameObject("PoolResources").transform, 10);
        _bubblePool = new PoolObjects<BubbleCollectable>(_bubblePrefab.PrefabBubble, new GameObject("PoolBubbles").transform, 10);
        
        var bubbles = _gameSave.GetData(SaveConstants.SavedBubbles, new List<BubbleStruct>());
        for (var i = 0; i < bubbles.Count; i++)
        {
            var bubble = bubbles[i];
            var position = new Vector3(bubble.X, bubble.Y, bubble.Z);
            var length = bubble.ResourceTypes.Length;
            var resources = Enumerable.Range(0, length).Select(x => (bubble.ResourceTypes[x], bubble.RecourcesCount[x])).ToArray();
            SpawnItem(position, true, resources: resources);
        }
        
        OnCreated.Invoke();
    }

    public CollectingResourceObject SpawnItem(Vector3 position, bool setToBubble = false, float? targetYPos = null, params (EResourceType, int)[] resources)
    {
        CollectingResourceObject item;

        //TODO: говно какое-то
        if (setToBubble)
        {
            item = _playerDeadResources.GetFree();
            item.Initialize(_gameResource, resources);
            item.transform.position = position;

            var bubble = CreateBubble(position);

            bubble.SetItem(item, targetYPos);
            AddBubble(bubble);
            bubble.OnDestroy += RemoveBubble;
        }
        else
        {
            item = _resourcesPool.GetFree();
            item.Initialize(_gameResource, resources);
            item.transform.position = position;
        }

        item.gameObject.SetActive(true);
        return item;
    }

    private Dictionary<BubbleCollectable, BubbleStruct> _bubbleStructs = new();

    private List<BubbleStruct> _saveData = new();

    public void AddBubble(BubbleCollectable bubbleCollectable)
    {
        if (_bubbleStructs.ContainsKey(bubbleCollectable) == false)
        {
            var position = bubbleCollectable.TargetPosition;
            var resources = bubbleCollectable.Resource.Resources.ToArray();
            var types = resources.Select(res => res.Item1).ToArray();
            var counts = resources.Select(res => res.Item2).ToArray();
            _bubbleStructs.Add(bubbleCollectable, new BubbleStruct()
            {
                ResourceTypes = types,
                RecourcesCount = counts, 
                X = position.x,
                Y = position.y,
                Z = position.z
            });
            _saveData.Add(_bubbleStructs[bubbleCollectable]);
        }
        _gameSave.SetData(SaveConstants.SavedBubbles, _saveData);
    }

    public void RemoveBubble(BubbleCollectable bubbleCollectable)
    {
        bubbleCollectable.OnDestroy -= RemoveBubble;
        if (_bubbleStructs.TryGetValue(bubbleCollectable, out var data))
        {
            _saveData.Remove(data);
        }
        _bubbleStructs.Remove(bubbleCollectable);
        _gameSave.SetData(SaveConstants.SavedBubbles, _saveData);
    }
    

    private BubbleCollectable CreateBubble(Vector3 position)
    {
        var bubbleInstance = _bubblePool.GetFree();
        bubbleInstance.transform.position = position;
        bubbleInstance.gameObject.SetActive(true);
        return bubbleInstance;
    }

    private void ValidateStates(EPlayerStates state, EntityPlayer entity)
    {
        switch (state)
        {
            case EPlayerStates.PlayerDeadInWater:
                var resources = _gameResource.LocalItems.ToArray();

                Debug.Log($"Count dead resource: {resources.Length}");

                if (resources.Any())
                {
                    foreach (var resource in resources)
                    {
                        _resourceService.TryRemoveLocal(resource.Item1, resource.Item2);
                    }

                    if (_currentLostResource != null)
                    {
                        _currentLostResource.FireCollectEvent();
                        _currentLostResource.OnCollection -= RemoveTarget;
                    }

                    var item = SpawnItem(entity.transform.position, true, _playerUpgradeSettings.GetValue<float>(EPlayerUpgradeType.MaxDepth), resources.ToArray());
                    _playerResources.Clear();

                    _currentLostResource = item;

                    _currentLostResource.OnCollection += RemoveTarget;

                    _indicatorControllerController.AddTarget(item.transform, new Vector3(Screen.width, Screen.height, 0f), 10, _tombIcon, new Color32(6, 16, 25, 255));
                }

                break;
        }
    }

    private void RemoveTarget(CollectingResourceObject item)
    {
        _indicatorControllerController.RemoveTarget(item.transform);
    }

    public void Dispose()
    {
        foreach (var bubble in _bubbleStructs)
        {
            bubble.Key.OnDestroy -= RemoveBubble;
        }
        if (_playerService != null)
        {
            _playerService.RemoveListener(ValidateStates);
        }
    }

    public int Priority { get; } = 1;
}