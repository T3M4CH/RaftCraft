using System;
using System.Collections.Generic;
using Game.Scripts.Core;
using Game.Scripts.Player.HeroPumping.Enums;
using Game.Scripts.Player.HeroPumping.Structs;
using UnityEngine;

namespace Game.Scripts.Player.HeroPumping
{
    [CreateAssetMenu(fileName = "PlayerUpgradesSettings", menuName = "Game/Upgrades")]
    public class PlayerUpgradesConfig : ScriptableObject, IWindowObject
    {
        public void InitializeDictionary()
        {
            _upgradesDictionary.Clear();
            _priceDictionary.Clear();

            if (_upgradesDictionary.Count == 0)
            {
                for (var i = 0; i < LevelUpgradesPairs.Length; i++)
                {
                    var pair = LevelUpgradesPairs[i];
                    _upgradesDictionary.TryAdd(pair.Key, pair.Value);
                }
            }

            if (_priceDictionary.Count == 0)
            {
                for (var i = 0; i < LevelPricePairs.Length; i++)
                {
                    var pair = LevelPricePairs[i];
                    _priceDictionary.TryAdd(pair.Key, pair.Value);
                }
            }
        }
    
        public SerializablePlayerUpgradesStruct GetValue(int level)
        {
            return _upgradesDictionary.TryGetValue(level, out var value) ? value : default;
        }

        public SerializablePlayerPriceStruct GetPrice(int level)
        {
            return _priceDictionary.TryGetValue(level, out var value) ? value : default;
        }

        public int GetCost(EPlayerUpgradeType type, int level)
        {
            foreach (var cost in UpgradeCosts)
            {
                if (cost.UpgradeType != type) continue;
                try
                {
                    return cost.Cost[level];
                }
                catch (Exception e)
                {
                    return int.MaxValue;
                }
            }

            return int.MaxValue;
        }

        public bool HaveMax(EPlayerUpgradeType type, int level)
        {
            foreach (var cost in UpgradeCosts)
            {
                if (cost.UpgradeType != type)
                {
                    continue;
                }

                return cost.Cost.Count <= level;
            }

            return true;
        }

        public string Patch => "Player/Upgrades";
        public object InstanceObject => this;
    
        public void CreateAsset()
        {
        }

        private readonly Dictionary<int, SerializablePlayerUpgradesStruct> _upgradesDictionary = new();
        private readonly Dictionary<int, SerializablePlayerPriceStruct> _priceDictionary = new();
        [field: SerializeField] public SerializablePair<int, SerializablePlayerUpgradesStruct>[] LevelUpgradesPairs;
        [field: SerializeField] public SerializablePair<int, SerializablePlayerPriceStruct>[] LevelPricePairs;
        [field: SerializeField] public List<UpgradeCost> UpgradeCosts = new List<UpgradeCost>();
    }


    [System.Serializable]
    public class UpgradeCost
    {
        public EPlayerUpgradeType UpgradeType;
        public List<int> Cost = new List<int>();
    }
}
