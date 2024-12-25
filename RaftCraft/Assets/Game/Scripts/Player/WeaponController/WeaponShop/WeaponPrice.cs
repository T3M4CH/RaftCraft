using Game.Scripts.ResourceController.Enums;
using UnityEngine;

namespace Game.Scripts.Player.WeaponController.WeaponShop
{
    [System.Serializable]
    public class WeaponPrice
    {
        [field: SerializeField] public WeaponId Id { get; private set; }
        [field: SerializeField] public EResourceType ItemType { get; private set; }
        [field: SerializeField] public int Cost { get; private set; }
        [field: SerializeField] public int DayUnLock { get; private set; }
    }
}
