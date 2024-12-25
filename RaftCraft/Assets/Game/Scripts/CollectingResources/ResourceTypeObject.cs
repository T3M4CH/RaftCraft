using System;
using Game.Scripts.ResourceController.Enums;
using UnityEngine;

namespace Game.Scripts.CollectingResources
{
    public class ResourceTypeObject : MonoBehaviour
    {
        [field: SerializeField] public  EResourceType Type { get; private set; }

        private void OnValidate()
        {
            gameObject.name = $"Resource_{Type}";
        }
    }
}
