using System.Collections.Generic;
using Game.GameBalanceCore.Scripts.BalanceValue;
using UnityEngine;

namespace Game.GameBalanceCore.scripts
{
    public class GameBalance : MonoBehaviour
    {
        public static GameBalance Instance;

        private GameBalanceConfig _balanceConfig; 

        private Dictionary<TypeValueBalance, BalanceFloat> _balanceValues;
        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            
            _balanceValues = new Dictionary<TypeValueBalance, BalanceFloat>()
            {
                {
                    TypeValueBalance.PirateHeals,
                    new BalanceFloat(0f, 300f, "Pirate Heals x", 0)
                },
                {
                    TypeValueBalance.PirateDamage,
                    new BalanceFloat(0f, 300f, "Pirate Damage x", 0)
                },
                {
                    TypeValueBalance.PirateSpeedMove,
                    new BalanceFloat(0f, 300f, "Pirate Speed Move x", 0)
                },
            };
            
            _balanceConfig = Resources.Load<GameBalanceConfig>("Balance/BalanceConfig");
        }

        public void UpdateConfigByDay(int day)
        {
            var config = _balanceConfig.GetConfig(day);
            
            InitializeConfig(config);
        }

        private void InitializeConfig(DataEnemyConfig config)
        {
            _balanceValues[TypeValueBalance.PirateHeals] = new BalanceFloat(0f, 10000f, "Pirate Heals x", config.MultipleHealsPirate);
            _balanceValues[TypeValueBalance.PirateDamage] = new BalanceFloat(0f, 10000f, "Pirate Damage x", config.MultipleDamagePirate);
            _balanceValues[TypeValueBalance.PirateSpeedMove] = new BalanceFloat(0f, 10000f, "Pirate Speed Move x", config.MultipleSpeedMovePirate);
        }

        public Dictionary<TypeValueBalance, BalanceFloat> GetBalances()
        {
            return _balanceValues;
        }

        public float GetBalanceValue(float defaultValue, TypeValueBalance typeValueBalance)
        {
            if (_balanceValues.TryGetValue(typeValueBalance, out var balance))
            {
                var result = balance.GetValue(defaultValue);
                return result;
            }

            return defaultValue;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            var obj = new GameObject("GameBalanceService");
            DontDestroyOnLoad(obj);
            obj.AddComponent<GameBalance>();
        }
    }
}
