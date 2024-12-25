using Game.Prefabs.NPC.Vendors;
using Game.Scripts.BattleMode;
using Game.Scripts.Player;
using Game.Scripts.Player.Spawners;
using Game.Scripts.Player.WeaponController;
using Game.Scripts.Player.WeaponController.Interface;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ResourceController.Interfaces;
using Reflex.Attributes;
using UnityEngine;

public class WeaponVendorHint : MonoBehaviour
{
    [SerializeField] private GameObject arrow;
    [SerializeField] private PlayerUpgradesVendor _upgradesVendor;

    private IBattleService _battleService;
    private IResourceService _resourceService;
    private IWeaponUpgradeService _weaponUpgradeService;

    [Inject]
    private void Construct(IPlayerService playerService, IResourceService resourceService, IWeaponUpgradeService weaponUpgradeService, IBattleService battleService)
    {
        _battleService = battleService;
        _battleService.OnChangeState += ValidateBattleState;
        _weaponUpgradeService = weaponUpgradeService;

        _upgradesVendor.OnEnterInteraction += Hide;

        _resourceService = resourceService;
        playerService.AddListener(ValidateStates);
    }

    private void ValidateBattleState(BattleState battleState)
    {
        switch (battleState)
        {
            case BattleState.Fight:
                Hide();
                break;
            case BattleState.Idle:
                //Show();
                break;
        }
    }

    private void Show()
    {
        arrow.SetActive(true);
    }

    private void Hide()
    {
        arrow.SetActive(false);
    }

    private void OnDestroy()
    {
        _battleService.OnChangeState -= ValidateBattleState; 
        _upgradesVendor.OnEnterInteraction -= Hide;
    }

    private void ValidateStates(EPlayerStates states, EntityPlayer _)
    {
        if (states == EPlayerStates.PlayerDead || states == EPlayerStates.PlayerDeadInRaft)
        {
            var coins = _resourceService.GetValue(EResourceType.CoinGold);
            if (_weaponUpgradeService.CostUpgrade(WeaponId.Pistol) <= coins)
            {
                Show();
            }
        }
    }
}