using System.Collections.Generic;
using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.Player.WeaponController.WeaponShop
{
    [CreateAssetMenu(fileName = "WeaponShopData", menuName = "SO/WeaponShopData")]
    public class WeaponShopData : ScriptableObject, IWindowObject
    {
        [SerializeField] private List<WeaponPrice> _weaponPrices = new List<WeaponPrice>();

        public bool TryGetTrice(int index, out WeaponPrice result)
        {
            if (index > _weaponPrices.Count - 1)
            {
                result = null;
                return false;
            }

            result = _weaponPrices[index];
            return true;
        }

        public string Patch => "WeaponShop/Prices";
        public void CreateAsset()
        {
        }
    }
}
