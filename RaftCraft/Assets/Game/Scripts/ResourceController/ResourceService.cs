using System;
using System.Collections;
using Game.Scripts.ResourceController.Interfaces;
using Game.Scripts.ResourceController.Enums;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Core.Interface;
using Game.Scripts.ResourceController;
using Game.Scripts.ResourceController.LocalPlayerResources;
using Game.Scripts.Saves;
using UnityEngine;

public class ResourceService : IResourceService, IGameObservable<ResourceItem>
{
    public event Action<EResourceType, int, TypeAddResource> OnEventAddResource;

    private readonly Dictionary<EResourceType, ResourceItem> _resources = new();
    private readonly GameSave _save;
    private readonly PlayerResources _playerResources;
    private readonly ResourceServiceSettings _settings;

    private readonly WaitForSeconds _delaySpawnUi;
    private List<IGameObserver<ResourceItem>> _observers;

    public ResourceService(GameSave save, ResourceServiceSettings settings)
    {
        _observers = new List<IGameObserver<ResourceItem>>();
        _resources = save.GetData(SaveConstants.PlayerResources, GetDefault());
        _save = save;
        _settings = settings;
        _delaySpawnUi = new WaitForSeconds(_settings.DurationSpawnIcon);
    }

    private Dictionary<EResourceType, ResourceItem> GetDefault()
    {
        var result = new Dictionary<EResourceType, ResourceItem>();
        foreach (var type in (EResourceType[])Enum.GetValues(typeof(EResourceType)))
        {
            if(type == EResourceType.Crystals) continue;
            result.TryAdd(type, new ResourceItem(HaveGlobal(type))
            {
                Type = type,
                Count = 0,
                TempCount = 0
            });
        }

        return result;
    }

    public static bool HaveGlobal(EResourceType type)
    {
        return type == EResourceType.CoinGold;
    }

    public int GetValue(EResourceType type)
    {
        if (_resources.TryGetValue(type, out var value))
        {
            return value.Count;
        }

        return 0;
    }

    public int GetValueLocal(EResourceType type)
    {
        if (_resources.TryGetValue(type, out var value))
        {
            return value.TempCount;
        }

        return 0;
    }

    public void Add(EResourceType type, int value)
    {
        if (_resources.TryGetValue(type, out var currentValue))
        {
            currentValue.Count += value;
        }
        else
        {
            _resources.Add(type, new ResourceItem(HaveGlobal(type))
            {
                Type = type,
                Count = value
            });
        }
        
        OnEventAddResource?.Invoke(type, value, TypeAddResource.Global);
        Save();
        Notify(_resources[type]);
    }

    public void AddLocal(EResourceType type, int value)
    {
        if (_resources.TryGetValue(type, out var currentValue))
        {
            currentValue.TempCount += value;
        }
        else
        {
            _resources.Add(type, new ResourceItem(HaveGlobal(type))
            {
                Type = type,
                TempCount = value
            });
        }

        OnEventAddResource?.Invoke(type, value, TypeAddResource.Local);
        Save();
        Notify(_resources[type]);
    }

    public bool TryRemove(EResourceType type, int value)
    {
        if (_resources.TryGetValue(type, out var currentValue))
        {
            if (currentValue.Count >= value)
            {
                _resources[type].Count -= value;
                Save();
                Notify(_resources[type]);
                return true;
            }
        }

        return false;
    }

    public bool TryRemoveLocal(EResourceType type, int value)
    {
        if (_resources.TryGetValue(type, out var currentValue))
        {
            if (currentValue.TempCount >= value)
            {
                _resources[type].TempCount -= value;
                Save();
                Notify(_resources[type]);
                return true;
            }
        }

        return false;
    }

    public bool HaveCount(EResourceType type, int value)
    {
        if (_resources.TryGetValue(type, out var currentValue))
        {
            return currentValue.Count >= value;
        }

        return false;
    }

    public void Save()
    {
        //TODO: возможно изменится 
        if (_save.HaveKey(SaveConstants.TutorialOne))
        {
            _save.SetData(SaveConstants.PlayerResources, _resources);
        }
    }

    public void AddObserver(IGameObserver<ResourceItem> gameObserver)
    {
        if (_observers.Contains(gameObserver))
        {
            return;
        }

        _observers.Add(gameObserver);
        NotifyTarget(gameObserver);
    }

    public void RemoveObserver(IGameObserver<ResourceItem> gameObserver)
    {
        _observers.Remove(gameObserver);
    }

    private void NotifyTarget(IGameObserver<ResourceItem> gameObserver)
    {
        foreach (var resource in _resources)
        {
            gameObserver.PerformNotify(resource.Value);
        }
    }

    public void Notify(ResourceItem data)
    {
        foreach (var observer in _observers)
        {
            if (observer == null)
            {
                continue;
            }

            observer.PerformNotify(data);
        }
    }
}