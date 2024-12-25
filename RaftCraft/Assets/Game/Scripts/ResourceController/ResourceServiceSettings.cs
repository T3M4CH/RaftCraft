using UnityEngine;

namespace Game.Scripts.ResourceController
{
    [System.Serializable]
    public struct ResourceServiceSettings
    {
        [field: SerializeField] public float DurationSpawnIcon { get; private set; }
    }
}
