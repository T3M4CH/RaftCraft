using System;
using Game.Scripts.ResourceController.Enums;
using UnityEngine;

namespace Game.Scripts.ResourceController.Structs
{
    [Serializable]
    public struct SerializableResourceValue
    {
        [field: SerializeField] public EResourceType ResourceType { get; private set; }
        [field: SerializeField] public int Count { get; private set; }
        [field: SerializeField] public int Price { get; private set; }
    }
}