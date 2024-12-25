using Cysharp.Threading.Tasks;
using Game.Scripts.Player.HeroPumping.Enums;
using Game.Scripts.Player.HeroPumping.Interfaces;
using Game.Scripts.Player.Spawners;
using Game.Scripts.Player.WeaponController;
using Game.Scripts.Player.WeaponController.Interface;
using Game.Scripts.Player.WeaponController.WeaponShop;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ResourceController.Interfaces;
using Newtonsoft.Json;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

public class MonoSceneTransitionTest : MonoBehaviour
{
    private IGameResourceService _resourceService;
    private IPlayerUpgradeService _service;
    private IWeaponUpgradeService _weaponUpgradeService;
    private IShopView _shopView;
    private IWeaponShop _weaponShop;
    [Inject]
    private void Construct(IGameResourceService resourceService, IPlayerService playerService, IWeaponUpgradeService weaponUpgradeService, IShopView shopView, IWeaponShop weaponShop)
    {
        _weaponUpgradeService = weaponUpgradeService;
        _service = playerService.UpgradeService;
        _resourceService = resourceService;
        _shopView = shopView;
        _weaponShop = weaponShop;
    }

    [Button]
    private void GetWeaponShop()
    {
        if (_shopView.TryGetCurrentWeapon(out var weapon))
        {
            Debug.Log($"{JsonConvert.SerializeObject(weapon)}:{_shopView.HaveUnLockCurrent()}:{_shopView.HaveBuyCurrent()}");
        }
    }

    [Button]
    private void BuyWeapon()
    {
        Debug.Log(_weaponShop.TryBuyWeapon());
    }

    [Button]
    private void GetCostWeapon(WeaponId id)
    {
        Debug.Log(_weaponUpgradeService.CostUpgrade(id));
    }
    
    [Button]
    public void Add(EResourceType type,int value)
    {
        _resourceService.Add(type, value);
    }


    [Button]
    public void Remove(EResourceType type, int value)
    {
        _resourceService.TryRemove(type, value);
    }

    
    [Button]
    public void ButtonUpgradePlayer(EPlayerUpgradeType upgradeType)
    {
        _service.Upgrade(upgradeType);
    }

    [Button]
    public void UpgradeWeapon(WeaponId id)
    {
        _weaponUpgradeService.Upgrade(id);
    }
    
}
