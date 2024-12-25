using System.Collections.Generic;
using Game.Scripts.ResourceController.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.NPC.Fish
{
    [System.Serializable]
    public class FishData
    {
        [field: SerializeField, MinValue(1)] public int Level { get; private set; }
        [field: SerializeField, MinValue(1)] public float Heals { get; private set; }
        [field: SerializeField, MinValue(0f)] public float SpeedMove { get; private set; }
        [field: SerializeField, MinValue(0f)] public float SpeedRotate { get; private set; }
        [field: SerializeField, MinValue(0f)] public float SpeedBoostMove { get; private set; }
        [field: SerializeField, MinValue(0f)] public float SpeedMoveAttack { get; private set; }
        [field: SerializeField, MinValue(0f)] public float DistanceUpdatePatch { get; private set; }
        [field: SerializeField, MinValue(0f)] public float TimeStayIdle { get; private set; }
        [field: SerializeField, MinValue(0f)] public float DistanceAttack { get; private set; }
        [field: SerializeField, MinValue(0f)] public float Damage { get; private set; }
        [field: SerializeField, MinValue(0f)] public float HealsGive { get; private set; }
        [field: SerializeField] public List<DropItems> DropItemsList { get; private set; }
    }

    [System.Serializable]
    public class DropItems
    {
        [field: SerializeField] public EResourceType ResourceType { get; set; }
        [field: SerializeField, MinValue(1)] public int Count { get; set; }
    }
}
