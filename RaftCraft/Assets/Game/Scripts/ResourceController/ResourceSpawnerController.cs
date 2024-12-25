using Game.Scripts.ResourceController.Structs;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.CollectingResources;
using Reflex.Attributes;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Days;
using Game.Scripts.Player;
using Game.Scripts.Player.HeroPumping.Enums;
using Game.Scripts.Player.Spawners;
using Game.Scripts.ResourceController.Interfaces;
using GTap.Analytics;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

public class ResourceSpawnerController : MonoBehaviour
{
    [SerializeField] private float _delay;
    [SerializeField] private float _maxDistance;
    [SerializeField] private float xOffsetMin;

    [SerializeField] private float xOffsetMax;

    //TODO: Вынести в отдельный конфиг
    [SerializeField] private ResourcesInBiome _resourcesPerDays;

    private int _resourceCollected;
    private float _currentTime;
    private float _playerXDelta;
    private float _previousPlayerXPos;
    private Transform _playerTransform;
    private IPlayerService _playerSpawner;
    private IResourceSpawner _resourceSpawner;
    private IDayService _dayController;
    private Queue<CollectingResourceObject> _collectables = new();
    private SerializableResourcesPerLevel[] _resourcesPerLevel;

    // [Inject]
    // private void Construct(IDayService dayController, IResourceSpawner resourceSpawner, IPlayerService playerSpawner)
    // {
    //     _playerSpawner = playerSpawner;
    //     _playerSpawner.AddListener(ValidatePlayer);
    //     _resourceSpawner = resourceSpawner;
    //
    //     _dayController = dayController;
    //
    //     //race condition
    //     _dayController.OnDayStart += ValidateSpawner;
    //
    //     _resourceSpawner.OnCreated += StartValidate;
    // }

    private void ValidateSpawner(int dayId)
    {
        // GtapAnalytics.ResourceCollected(dayId - 1, _dayController.AttemptsCount, EResourceCollected.WaterResources, _resourceCollected);
        //
        // _resourceCollected = 0;
        //
        // PerformSpawn(dayId);
    }

    private void PerformSpawn(int dayId)
    {
        var resourcesPerDay = _resourcesPerDays.ResourcesPerDay.FirstOrDefault(pair => pair.DayId == dayId);
        if (resourcesPerDay.ResourcesPerLevel == null)
        {
            _resourcesPerLevel = _resourcesPerDays.ResourcesPerDay[^1].ResourcesPerLevel;
        }
        else
        {
            _resourcesPerLevel = resourcesPerDay.ResourcesPerLevel;
        }

        Clear();

        Spawn();
    }

    private void RemoveItem(CollectingResourceObject resource)
    {
        _resourceCollected += 1;

        resource.OnCollection -= RemoveItem;

        switch (resource.ResourceType)
        {
            case EResourceType.CoinBlue:

                break;
            case EResourceType.Wood:
                break;
            case EResourceType.Crystals:
                break;

        }

        RemoveResource(resource.transform);
    }

    private void ValidatePlayer(EPlayerStates state, EntityPlayer entityPlayer)
    {
        if (state == EPlayerStates.SpawnPlayer)
        {
            _playerTransform = entityPlayer.transform;
        }
    }

    private void Update()
    {
        if (_playerTransform == null) return;

        _playerXDelta = _playerTransform.position.x - _previousPlayerXPos;
        _playerXDelta = Mathf.Sign(_playerXDelta);

        _currentTime += Time.deltaTime;

        if (_currentTime > _delay)
        {
            _currentTime = 0;
            RespawnResources();
        }

        _previousPlayerXPos = _playerTransform.position.x;
    }

    private void RespawnResources()
    {
        for (var i = 0; i < _resourcesPerLevel.Length; i++)
        {
            var depthInfo = _resourcesPerLevel[i];

            for (var t = 0; t < depthInfo.Transforms.Count; t++)
            {
                var resourceTransform = depthInfo.Transforms[t];

                if (resourceTransform == null) continue;

                if (Vector3.Distance(_playerTransform.position, resourceTransform.position) > _maxDistance)
                {
                    resourceTransform.position = new Vector3(_playerTransform.position.x + Random.Range(xOffsetMin, xOffsetMax) * _playerXDelta, Random.Range(depthInfo.yMin, depthInfo.yMax), 0);
                }
            }
        }
    }

    private void RemoveResource(Transform resourceTransform)
    {
        for (var i = 0; i < _resourcesPerLevel.Length; i++)
        {
            var depthInfo = _resourcesPerLevel[i];

            if (depthInfo.Transforms.Contains(resourceTransform))
            {
                depthInfo.Transforms.Remove(resourceTransform);
                break;
            }
        }
    }

    private void Clear()
    {
        foreach (var collectable in _collectables)
        {
            collectable.OnCollection -= RemoveItem;
            collectable.gameObject.SetActive(false);
        }

        _collectables.Clear();
    }

    [Button]
    private void DisplayCount()
    {
        for (var i = 0; i < _resourcesPerLevel.Length; i++)
        {
            Debug.Log(_resourcesPerLevel[i].Transforms.Count);
        }
    }

    private void StartValidate()
    {
        ValidateSpawner(_dayController.CurrentDay);
    }

    //TODO: Вызывать спавн по ивенту, стоит переписать на пул
    private void Spawn()
    {
        for (var i = 0; i < _resourcesPerLevel.Length; i++)
        {
            ref var levelInfo = ref _resourcesPerLevel[i];
            var transforms = levelInfo.Transforms;

            for (var j = 0; j < levelInfo.Resources.Length; j++)
            {
                var resource = levelInfo.Resources[j];

                for (var t = 0; t < resource.Count; t++)
                {
                    var item = _resourceSpawner.SpawnItem(new Vector3(Random.Range(-xOffsetMax, xOffsetMax), Random.Range(levelInfo.yMax, levelInfo.yMin) - t, 0), resources: (resource.ResourceType, resource.Price));
                    item.OnCollection += RemoveItem;
                    _collectables.Enqueue(item);

                    transforms.Add(item.transform);
                }
            }

            levelInfo.Transforms = transforms;
        }
    }

    // private void OnDestroy()
    // {
    //     _playerSpawner.RemoveListener(ValidatePlayer);
    // }
}