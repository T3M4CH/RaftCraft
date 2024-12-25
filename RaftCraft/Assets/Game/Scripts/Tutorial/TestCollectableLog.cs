using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ResourceController.Interfaces;
using Reflex.Attributes;
using UnityEngine;

public class TestCollectableLog : MonoBehaviour
{
    private IResourceService _resourceService;

    [Inject]
    private void Construct(IResourceService resourceService)
    {
        _resourceService = resourceService;
    }

    public void Collect()
    {
        _resourceService.Add(EResourceType.Wood, 1);
        Destroy(gameObject);
    }
}