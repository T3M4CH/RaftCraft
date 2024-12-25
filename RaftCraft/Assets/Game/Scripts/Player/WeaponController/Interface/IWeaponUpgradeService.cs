using System;

namespace Game.Scripts.Player.WeaponController.Interface
{
    public interface IWeaponUpgradeService
    {
        public event Action OnUpgrade;
        public void Upgrade(WeaponId id);
        public int CostUpgrade(WeaponId id);
    }
}