using System;
using Game.Scripts.InteractiveObjects.Interfaces;
using Game.Scripts.ResourceController.Interfaces;
using Game.Scripts.InteractiveObjects;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ShopController;
using GTapSoundManager.SoundManager;
using Reflex.Attributes;
using UnityEngine;

public class MonoNetInteractor : MonoBehaviour, IInteraction
{
    public event Action<bool> OnRemovedResource;

    [SerializeField] private float multiplier;
    [SerializeField] private MonoNetController netController;

    private int _endId;
    private int _startId;
    private float _currentPitch = 1f;
    private IResourceService _resourceService;
    private ShopData _shopData;

    [Inject]
    private void Construct(IResourceService resourceService)
    {
        _resourceService = resourceService;
        _shopData = Resources.Load<ShopData>("Shop/ShopFish");

        _startId = (int)EResourceType.FishLvl1;
        _endId = (int)EResourceType.FishLvl10;
    }

    public void Action()
    {
        _currentPitch *= multiplier;
        _currentPitch = Mathf.Min(_currentPitch, 1.5f);

        if (!RemoveFish())
        {
            ExitInteraction();
        }
    }

    public void EnterInteraction()
    {
    }

    public void ExitInteraction()
    {
        _currentPitch = 1;
    }

    private bool RemoveFish()
    {
        for (var i = _startId; i <= _endId; i++)
        {
            var resourceType = (EResourceType)i;
            var price = _shopData.Price[i - _startId].CountOutput;
            if (_resourceService.TryRemove(resourceType, 1))
            {
                _resourceService.TryRemove(EResourceType.UniversalFish, price);
                netController.AddFish(i - _startId, _currentPitch);
                OnRemovedResource?.Invoke(true);
                return true;
            }
        }

        OnRemovedResource?.Invoke(false);
        return false;
    }

    public bool Interaction => true;
    public bool IsAbleEverywhere => true;
    public float DelayAction => 0.2f;
    public InteractionType CurrentTypeInteraction { get; }
}