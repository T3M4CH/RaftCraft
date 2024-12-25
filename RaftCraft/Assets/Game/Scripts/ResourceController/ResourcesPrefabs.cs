using Game.Scripts.CollectingResources;
using UnityEngine;

namespace Game.Scripts.ResourceController
{
    [System.Serializable]
    public struct ResourcesPrefabs
    {
        [field: SerializeField] public BubbleCollectable PrefabBubble { get; private set; }
        [field: SerializeField] public CollectingResourceObject PrefabResource { get; private set; }
    }
}
