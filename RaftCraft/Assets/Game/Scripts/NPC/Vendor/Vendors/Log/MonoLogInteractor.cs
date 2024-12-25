using System;
using System.Collections.Generic;
using System.Linq;
using Game.Prefabs.NPC.Vendors.Log;
using Game.Scripts.InteractiveObjects.Interfaces;
using Game.Scripts.InteractiveObjects;
using Game.Scripts.Pool;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ResourceController.Interfaces;
using Game.Scripts.Saves;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

public class MonoLogInteractor : MonoBehaviour, IInteraction
{
    [SerializeField] private float delay;
    [SerializeField] private float yOffset;
    [SerializeField] private float multiplier;
    [SerializeField] private Transform[] places;
    [SerializeField] private MonoLog logPrefab;

    private int _currentCount;
    private float _currentDelay;
    private GameSave _gameSave;
    private PoolObjects<MonoLog> _logPool;
    private List<MonoLog> _logList = new();
    private IResourceService _resourceService;

    [Inject]
    private void Construct(GameSave save, IResourceService resourceService)
    {
        _gameSave = save;
        _resourceService = resourceService;
    }

    public MonoLog TakeWood()
    {
        if (!_logList.Any()) return null;

        var log = _logList[^1];
        log.gameObject.SetActive(false);
        _logList.Remove(log);
        _currentCount -= 1;

        return log;
    }

    private bool AddWood()
    {
        if (_resourceService.TryRemove(EResourceType.Wood, 1))
        {
            Spawn();

            return true;
        }

        return false;
    }

    [Button]
    private void Spawn()
    {
        var id = _currentCount % 8;
        var floor = _currentCount / 8;
        var place = places[id];
        var logInstance = _logPool.GetFree(true);
        var log = logInstance.transform;
        log.position = place.position;
        log.rotation = place.rotation;
        var position = log.position;
        position.y += floor * yOffset;
        log.position = position;
        log.SetParent(transform);
        
        _logList.Add(logInstance);

        _currentCount += 1;

        _gameSave.SetData(SaveConstants.LogsOnTable, _currentCount);
    }

    public void Action()
    {
        _currentDelay *= multiplier;

        if (!AddWood())
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

    private void Start()
    {
        _logPool = new PoolObjects<MonoLog>(logPrefab, transform, 5);

        var targetCount = _gameSave.GetData(SaveConstants.LogsOnTable, 0);
        for (int i = 0; i < targetCount; i++)
        {
            Spawn();
        }
    }

    public bool Interaction => true;
    public bool IsAbleEverywhere => true;
    public float DelayAction => _currentDelay;
    public InteractionType CurrentTypeInteraction { get; }
}