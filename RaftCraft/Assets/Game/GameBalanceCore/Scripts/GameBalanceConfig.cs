using System;
using Game.Scripts.Core;
using Game.Scripts.Extension;
using UnityEngine;

namespace Game.GameBalanceCore.scripts
{
    [CreateAssetMenu(menuName = "GameBalance/Config",fileName = "BalanceConfig")]
    public class GameBalanceConfig : ScriptableObject, IWindowObject
    {
        public string Patch => "GameBalance/Config";
        public void CreateAsset()
        {
            
        }
        
        [SerializeField] private DataEnemyConfig[] _configs;

        public DataEnemyConfig GetConfig(int day)
        {
            var result = _configs.Peek();
            foreach (var config in _configs)
            {
                if (config.Day >= day)
                {
                    result = config;
                    break;
                }
            }

            return result;
        }
    }

    [Serializable]
    public struct DataEnemyConfig
    {
        public int Day;
        public float MultipleHealsPirate;
        public float MultipleDamagePirate;
        public float MultipleSpeedMovePirate;
    }
}