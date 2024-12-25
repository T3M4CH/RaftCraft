using System;
using UnityEngine;

namespace Game.Scripts.Health
{
    [Serializable]
    public struct SerializableDamageEffectSettings
    {
        [field: SerializeField] public Transform Parent { get; private set; }
        [field: SerializeField] public RectTransform Panel { get; private set; }
    }
}