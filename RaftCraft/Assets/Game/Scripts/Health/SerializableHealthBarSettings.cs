using System;
using UnityEngine;

namespace Game.Scripts.Health
{
    [Serializable]
    public struct SerializableHealthBarSettings
    {
        [field: SerializeField] public Transform Parent { get; private set; }
        [field: SerializeField] public RectTransform Panel { get; private set; }
        [field: SerializeField] public MonoHealthBar HealthBarPrefab { get; private set; }
        [field: SerializeField] public float MaxDistance { get; private set; }
    }
}