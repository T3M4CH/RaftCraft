using System;
using UnityEngine;

namespace Game.Scripts.NPC.Fish.Configs.Structs
{
    [Serializable]
    public struct SerializableFishPerDay
    {
        [field: SerializeField] public int DayId { get; set; }
        [field: SerializeField] public SerializableFishPerLevel[] FishPerLevels { get; set; }
    }
}