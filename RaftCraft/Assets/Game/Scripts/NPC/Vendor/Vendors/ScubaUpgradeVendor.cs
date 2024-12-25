using Game.Scripts.Player.HeroPumping.Interfaces;
using Game.Scripts.BattleMode;
using Game.Scripts.Player.HeroPumping.Enums;
using Game.Scripts.Player.Spawners;
using Game.Scripts.UI.WindowManager;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Prefabs.NPC.Vendors
{
    public class ScubaUpgradeVendor : PlayerUpgradesVendor
    {
        [SerializeField] private GameObject mask;
        [SerializeField] private GameObject flippers;
        [SerializeField] private GameObject suit;
        [SerializeField] private GameObject balloon;
    
        private IPlayerService _playerService;
        private IPlayerUpgradeSettings _playerUpgradeSettings;

        [Inject]
        private void Construct(IPlayerService playerService, IBattleService battleService)
        {
            _playerService = playerService;
        }

        private void Start()
        {
            _playerUpgradeSettings = _playerService.UpgradeSettings;
            _playerUpgradeSettings.OnUpgrade += ValidateClothes;
            ValidateClothes(EPlayerUpgradeType.Oxygen);
        }

        private void ValidateClothes(EPlayerUpgradeType _)
        {
            var level = _playerUpgradeSettings.GetLevel(EPlayerUpgradeType.Oxygen);
            mask.SetActive(level >= 2);
            flippers.SetActive(level >= 4);
            suit.SetActive(level >= 6);
            balloon.SetActive(level >= 8);
        }

        private void OnDestroy()
        {
            _playerUpgradeSettings.OnUpgrade -= ValidateClothes;
        }
    }
}