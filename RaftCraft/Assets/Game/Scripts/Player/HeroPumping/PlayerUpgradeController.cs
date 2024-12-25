using Game.Scripts.Player.HeroPumping.Interfaces;
using Game.Scripts.Player.HeroPumping.Enums;
using System.Collections.Generic;
using Game.Scripts.Saves;
using System;
using Game.Scripts.Days;
using Game.Scripts.Player.HeroPumping.Structs;
using GTap.Analytics;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Scripts.Player.HeroPumping
{
    public class PlayerUpgradeController : IPlayerUpgradeSettings, IPlayerUpgradeService, IDisposable
    {
        public PlayerUpgradeController(PlayerUpgradesConfig playerUpgradesConfig)
        {
            _upgradesConfig = playerUpgradesConfig;
            _upgradesConfig.InitializeDictionary();

            Debug.Log($"{JsonConvert.SerializeObject(_upgradeLevels)}");
        }

        public void Load(GameSave gameSave)
        {
            _save = gameSave;
            _upgradeLevels = _save.GetData(SaveConstants.PlayerUpgrades, new Dictionary<EPlayerUpgradeType, int>
            {
                { EPlayerUpgradeType.Capacity, 1 },
                { EPlayerUpgradeType.MaxDepth, 1 },
                { EPlayerUpgradeType.MaxHealth, 1 },
                { EPlayerUpgradeType.RaftSpeed, 1 },
                { EPlayerUpgradeType.WaterSpeed, 1 },
                { EPlayerUpgradeType.FishLevel, 1 },
                { EPlayerUpgradeType.Oxygen, 1 },
            });
        }

        public event Action<EPlayerUpgradeType> OnUpgrade;

        private readonly PlayerUpgradesConfig _upgradesConfig;

        private GameSave _save;
        private Dictionary<EPlayerUpgradeType, int> _upgradeLevels;

        public T GetValue<T>(EPlayerUpgradeType playerUpgradeType, int? level = null) where T : IConvertible
        {
            var currentLevel = _upgradeLevels[playerUpgradeType];

            if (level.HasValue)
            {
                currentLevel = level.Value;
            }

            var upgradeStruct = _upgradesConfig.GetValue(currentLevel);

            if (upgradeStruct.Equals(default))
            {
                throw new Exception($"UpgradeStruct is empty at level {currentLevel}");
            }

            T value;
            switch (playerUpgradeType)
            {
                case EPlayerUpgradeType.Capacity:
                    value = (T)Convert.ChangeType(upgradeStruct.Capacity, typeof(T));
                    return value;
                case EPlayerUpgradeType.MaxDepth:
                    value = (T)Convert.ChangeType(upgradeStruct.MaxDepth, typeof(T));
                    return value;
                case EPlayerUpgradeType.MaxHealth:
                    value = (T)Convert.ChangeType(upgradeStruct.MaxHealth, typeof(T));
                    return value;
                case EPlayerUpgradeType.RaftSpeed:
                    value = (T)Convert.ChangeType(upgradeStruct.RaftSpeed, typeof(T));
                    return value;
                case EPlayerUpgradeType.WaterSpeed:
                    value = (T)Convert.ChangeType(upgradeStruct.WaterSpeed, typeof(T));
                    return value;
                case EPlayerUpgradeType.FishLevel:
                    value = (T)Convert.ChangeType(upgradeStruct.FishLevels, typeof(T));
                    return value;
                case EPlayerUpgradeType.Oxygen:
                    value = (T)Convert.ChangeType(upgradeStruct.Oxygen, typeof(T));
                    return value;
                default:
                    throw new ArgumentOutOfRangeException(nameof(playerUpgradeType), playerUpgradeType, null);
            }
        }

        public int GetPrice(EPlayerUpgradeType type)
        {
            if (type != EPlayerUpgradeType.RaftSpeed && type != EPlayerUpgradeType.MaxHealth)
            {
                var currentLevel = _upgradeLevels[type];

                var price = _upgradesConfig.GetCost(type, currentLevel);

                return price;
            }
            else
            {
                var currentLevel = _upgradeLevels[type];

                var price = _upgradesConfig.GetPrice(currentLevel);

                switch (type)
                {
                    case EPlayerUpgradeType.MaxHealth:
                        return price.MaxHealth;
                    case EPlayerUpgradeType.RaftSpeed:
                        return price.RaftSpeed;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }
            }
        }
        

        public int GetLevel(EPlayerUpgradeType type)
        {
            return _upgradeLevels[type];
        }

        /// <summary>
        /// analytics scuba event only
        /// </summary>
        public void Upgrade(params EPlayerUpgradeType[] upgradeTypes)
        {
            var level = 0;
            foreach (var type in upgradeTypes)
            {
                _upgradeLevels[type] += 1;
                level = _upgradeLevels[type];
                OnUpgrade.Invoke(type);
            }

            GtapAnalytics.Upgrade(level, "Aqualang");

            _save.SetData(SaveConstants.PlayerUpgrades, _upgradeLevels);
        }

        public bool Upgrade(EPlayerUpgradeType type, int? value = null)
        {
            if (value != null)
            {
                if (_upgradesConfig.LevelUpgradesPairs.Length < value + 1) return false;
                _upgradeLevels[type] = value.Value;
            }
            else
            {
                if (_upgradesConfig.LevelUpgradesPairs.Length < _upgradeLevels[type] + 1) return false;
                _upgradeLevels[type] += 1;
            }

            var level = _upgradeLevels[type];


            OnUpgrade?.Invoke(type);
            GtapAnalytics.Upgrade(level, type.ToString());

            _save.SetData(SaveConstants.PlayerUpgrades, _upgradeLevels);

            return true;
        }

        public bool HaveMax(EPlayerUpgradeType playerUpgradeType)
        {
            return _upgradesConfig.HaveMax(playerUpgradeType, _upgradeLevels[playerUpgradeType]);
        }

        public void Dispose()
        {
            OnUpgrade = null;
        }

        public bool IsMaxLevelPlayer => _upgradesConfig.LevelUpgradesPairs.Length < _upgradeLevels[EPlayerUpgradeType.MaxHealth];
        public bool IsMaxLevelAqua => _upgradesConfig.LevelUpgradesPairs.Length < _upgradeLevels[EPlayerUpgradeType.FishLevel];
    }
}