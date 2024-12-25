using System;
using Game.Scripts.Player.HeroPumping.Enums;
using Game.Scripts.Player.HeroPumping.Interfaces;
using Game.Scripts.Player.Spawners;
using Game.Scripts.Player.WeaponController.Interface;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Scripts.Player
{
    public class PlayerFeedbackUpgrades : MonoBehaviour
    {
        [SerializeField] private GameObject _effect;
        [SerializeField] private Vector3 _position;
        
        private GameObject _currentEffect;

        private IPlayerService _playerService;
        private IPlayerUpgradeService _upgradeService;
        private IWeaponUpgradeService _weaponUpgradeService;
        
        [Inject]
        private void Construct(IPlayerService playerService, IWeaponUpgradeService weaponUpgradeService)
        {
            _playerService = playerService;
            _upgradeService = _playerService.UpgradeService;
            _weaponUpgradeService = weaponUpgradeService;
        }

        private void Start()
        {
            _playerService.AddListener(OnChange);
            _upgradeService.OnUpgrade += UpgradeServiceOnOnUpgrade;
            _weaponUpgradeService.OnUpgrade += WeaponUpgrade;
        }

        private void WeaponUpgrade()
        {
            if (_currentEffect == null)
            {
                return;
            }
            
            _currentEffect.SetActive(false);
            _currentEffect.SetActive(true);
        }

        private void OnChange(EPlayerStates states, EntityPlayer player)
        {
            switch (states)
            {
                case EPlayerStates.NotPlayer:
                    break;
                case EPlayerStates.SpawnPlayer:
                    _currentEffect = Instantiate(_effect, player.transform);
                    _currentEffect.transform.localPosition = _position;
                    _currentEffect.SetActive(false);
                    break;
                case EPlayerStates.PlayerInRaft:
                    break;
                case EPlayerStates.PlayerInWater:
                    break;
                case EPlayerStates.PlayerInBattle:
                    break;
                case EPlayerStates.PlayerDead:
                    break;
                case EPlayerStates.PlayerDeadInRaft:
                    break;
                case EPlayerStates.PlayerDeadInWater:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(states), states, null);
            }
        }

        private void UpgradeServiceOnOnUpgrade(EPlayerUpgradeType obj)
        {
            if (_currentEffect == null)
            {
                return;
            }
            
            _currentEffect.SetActive(false);
            _currentEffect.SetActive(true);
        }

        private void OnDestroy()
        {
            _playerService.RemoveListener(OnChange);
            _upgradeService.OnUpgrade -= UpgradeServiceOnOnUpgrade;
            _weaponUpgradeService.OnUpgrade -= WeaponUpgrade;
        }
    }
}
