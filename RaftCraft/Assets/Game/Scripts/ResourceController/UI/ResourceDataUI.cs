using UnityEngine;

namespace Game.Scripts.ResourceController.UI
{
    [System.Serializable]
    public struct ResourceDataUI
    {
        [field: SerializeField] public ResourceObjectUI PrefabUI { get; private set; }
        [field: SerializeField] public ResourceObjectUI PrefabFishUI { get; private set; }
        [field: SerializeField] public RectTransform ParentWindow { get; private set; }
        [field: SerializeField] public RectTransform TargetMoneyPanel { get; private set; }
        [field: SerializeField] public RectTransform TargetResourcePanel { get; private set;}
        [field: SerializeField] public RectTransform TargetFishPanel { get; private set;}
        [field: SerializeField] public RectTransform TargetDiamondPane { get; private set; }
        [field: SerializeField] public Vector3 OffsetTargetPosition { get; private set; }
    }
}
