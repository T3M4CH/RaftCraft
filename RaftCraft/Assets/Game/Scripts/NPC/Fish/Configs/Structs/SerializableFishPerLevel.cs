using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.NPC.Fish.Configs.Structs
{
    [Serializable]
    public struct SerializableFishPerLevel
    {
        [LabelText("Depth Min/Max"), HorizontalGroup("Depth"), MaxValue(0)]
        public float yMax;

        [HideLabel, HorizontalGroup("Depth"), MaxValue(0)] public float yMin;

        [field: SerializeField, MinValue(1)] public SerializableFishLevelsCount[] FishCount { get;  set; }
    }

    [Serializable]
    public struct SerializableFishLevelsCount
    {
        [field: SerializeField, MinValue(1)] public int Count { get;  set;  }
        [field: SerializeField, MinValue(1)] public int FishLevel { get; set; }
    }
}