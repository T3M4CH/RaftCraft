using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.ResourceController.Structs
{
    [CreateAssetMenu(fileName = "ResourcesInBiome", menuName = "Resources/ResourcesInBiome")]
    public class ResourcesInBiome : ScriptableObject, IWindowObject
    {
        [field: SerializeField] public SerializableResourcesPerDay[] ResourcesPerDay { get; private set; }

        public string Patch => $"Resources/{name}";
        public object InstanceObject => this;
        public void CreateAsset()
        {
        }
    }
}