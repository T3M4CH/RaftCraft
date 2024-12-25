using System;
using Game.Scripts.Player.HeroPumping.Enums;

namespace Game.Scripts.Player.HeroPumping.Interfaces
{
    public interface IPlayerUpgradeService
    {
        public event Action<EPlayerUpgradeType> OnUpgrade;
        bool IsMaxLevelPlayer { get; }
        bool IsMaxLevelAqua { get; }
        int GetPrice(EPlayerUpgradeType type);
        void Upgrade(params EPlayerUpgradeType[] upgradeTypes);
        bool Upgrade(EPlayerUpgradeType type, int? value = null);
        bool HaveMax(EPlayerUpgradeType playerUpgradeType);
    }
}