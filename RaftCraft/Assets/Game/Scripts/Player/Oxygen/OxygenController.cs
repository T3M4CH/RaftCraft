using Game.Scripts.Player.HeroPumping.Interfaces;
using Game.Scripts.Player.HeroPumping.Enums;
using UnityEngine;

namespace Game.Scripts.Player.Oxygen
{
    public class OxygenController : MonoBehaviour
    {
        [SerializeField] private float delay;
        [SerializeField] private float damage;
        [SerializeField] private EntityPlayer _player;

        private float _yThreshold;
        private float _currentTime;
        private float _maxOxygenValue;
        private Transform _transform;
        private IPlayerUpgradeSettings _upgradeSettings;

        private void Start()
        {
            _transform = _player.Hips;
            
            _yThreshold = _player.Settings.MaxHeightMoveWater;
            _upgradeSettings = _player.PlayerSettings;
            _upgradeSettings.OnUpgrade += ValidateUpgrade;
            
            ValidateUpgrade(EPlayerUpgradeType.Oxygen);
            
            CurrentOxygenValue = _maxOxygenValue;
        }

        private void ValidateUpgrade(EPlayerUpgradeType upgradeType)
        {
            if (upgradeType == EPlayerUpgradeType.Oxygen)
            {
                _maxOxygenValue = _upgradeSettings.GetValue<float>(EPlayerUpgradeType.Oxygen);
            }
        }

        private void Update()
        {
            if (_transform.position.y > -1)
            {
                CurrentOxygenValue += Time.deltaTime * 10;

                CurrentOxygenValue = Mathf.Min(_maxOxygenValue, CurrentOxygenValue);
            }
            else
            {
                CurrentOxygenValue -= Time.deltaTime;
                CurrentOxygenValue = Mathf.Max(CurrentOxygenValue, -0.01f);

                if (CurrentOxygenValue <= 0)
                {
                    _currentTime -= Time.deltaTime;

                    if (_currentTime < 0)
                    {
                        _currentTime = delay;
                        _player.TakeDamage(damage);
                    }
                }
            }
        }

        public float Ratio => Mathf.Max(0, CurrentOxygenValue / _maxOxygenValue);
        public float CurrentOxygenValue { get; private set; }
    }
}