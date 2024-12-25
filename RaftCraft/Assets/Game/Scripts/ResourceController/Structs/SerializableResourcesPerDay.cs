using System;
using UnityEngine;

namespace Game.Scripts.ResourceController.Structs
{
    [Serializable]
    public struct SerializableResourcesPerDay
    {
        [field: SerializeField] public int DayId { get; private set; }
        [field: SerializeField] public SerializableResourcesPerLevel[] ResourcesPerLevel { get; private set; }
    }
}