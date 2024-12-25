using System;
using Game.Scripts.Core.Interface;

namespace Game.Scripts.Player.WeaponController.WeaponShop
{
    public interface IShopView : IGameObservable<WeaponPrice>
    {
        public event Action<WeaponPrice> OnBuyWeapon; 
        public bool TryGetCurrentWeapon(out WeaponPrice result);
        public bool HaveUnLockCurrent();

        public bool HaveBuyCurrent();
    }
}