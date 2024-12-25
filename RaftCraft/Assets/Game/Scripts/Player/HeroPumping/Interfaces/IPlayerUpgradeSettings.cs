using Game.Scripts.Player.HeroPumping.Enums;
using System;

namespace Game.Scripts.Player.HeroPumping.Interfaces
{
    public interface IPlayerUpgradeSettings
    {
        event Action<EPlayerUpgradeType> OnUpgrade;
        
        T GetValue<T>(EPlayerUpgradeType playerUpgradeType, int? level = null) where T : IConvertible;
        int GetLevel(EPlayerUpgradeType type);

    }
}