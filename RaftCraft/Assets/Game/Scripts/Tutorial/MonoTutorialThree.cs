using System;
using Game.Scripts.UI.WindowManager;
using Game.Scripts.Player.Spawners;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ResourceController.Interfaces;
using Game.Scripts.ShopVendor;
using Game.Scripts.Saves;
using UnityEngine;

public class MonoTutorialThree : MonoTutorialBase
{
    [SerializeField] private Transform upgradeText;
    
    private ShopVendorController _shopVendor;
    private IResourceService _resourceService;

    public override void Initialize(GameSave gameSave, IPlayerService playerSpawner, WindowManager windowManager, object extraParams = null)
    {
        _resourceService = (IResourceService)extraParams;
        
        ShowBattleButton(false);
    }

    protected override void Complete()
    {
        base.Complete();
        
        Destroy(gameObject);
    }

    private void Start()
    {
        var difference = 3 - _resourceService.GetValue(EResourceType.CoinGold);
        _resourceService.Add(EResourceType.CoinGold, difference);
        
        _shopVendor = FindObjectOfType<WeaponVendorHint>().GetComponentInChildren<ShopVendorController>();
        _shopVendor.OnBuyEvent += Complete;

        if (_shopVendor.State == ShopVendorController.ShopState.UnLock)
        {
            Complete();
            return;
        }

        upgradeText.gameObject.SetActive(true);
        upgradeText.transform.position = _shopVendor._viewCost.transform.position - Vector3.up;
    }

    private void OnDestroy()
    {
        _shopVendor.OnBuyEvent -= Complete;
    }
}
