using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using System;

namespace Game.Scripts.ResourceController.Structs
{
    [Serializable]
    public struct SerializableResourcesPerLevel
    {
        [LabelText("Depth Min/Max"), HorizontalGroup("Depth")] public float yMax;

        [HideLabel, HorizontalGroup("Depth")] public float yMin;
        
        [field: SerializeField] public SerializableResourceValue[] Resources { get; private set; }

        [HideInInspector] public List<Transform> Transforms;
    }
}