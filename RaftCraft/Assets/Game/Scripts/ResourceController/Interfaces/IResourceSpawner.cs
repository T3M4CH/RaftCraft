using System;
using Game.Scripts.CollectingResources;
using Game.Scripts.ResourceController.Enums;
using UnityEngine;

namespace Game.Scripts.ResourceController.Interfaces
{
    public interface IResourceSpawner
    {
        event Action OnCreated;
        CollectingResourceObject SpawnItem(Vector3 position, bool setToBubble = false, float? targetYPos = null, params (EResourceType, int)[] resources);
    }
}