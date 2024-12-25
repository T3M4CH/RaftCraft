using System;
using Game.Scripts.Core.Interface;
using Game.Scripts.Player;
using Game.Scripts.Player.HeroPumping.Interfaces;
using Game.Scripts.Player.Spawners;
using Game.Scripts.Player.WeaponController.WeaponInventory.UI;
using Game.Scripts.ResourceController;
using Game.Scripts.UI.WindowManager;
using Game.Scripts.UI.WindowVendors.Enums;
using Reflex.Attributes;
using UnityEngine;
using IResourceService = Game.Scripts.ResourceController.Interfaces.IResourceService;

namespace Game.Scripts.UI.WindowVendors
{
    public class SelectorWindow : UIWindow
    {
        [SerializeField] private WeaponUpgradeElement weaponPanel;
        [SerializeField] private PlayerUpgradeElement playerUpgrade;
        [SerializeField] private PedestalWeaponElement pedestalWeapon;
        [SerializeField] private WeaponInventoryView _weaponInventory;
        [SerializeField] private AqualangUpgradeElement aqualangUpgradeElement;

        private IPlayerUpgradeService _playerUpgradeService;
        private IPlayerUpgradeSettings _playerUpgradeSettings;
        private IResourceService _resourceService;
        private IGameObservable<ResourceItem> _observableItem;
        private IPlayerService _playerService;
        private EPlayerStates _currentState;

        private EPlayerStates State
        {
            get => _currentState;

            set
            {
                _currentState = value;
                switch (_currentState)
                {
                    case EPlayerStates.NotPlayer:
                        break;
                    case EPlayerStates.SpawnPlayer:
                        break;
                    case EPlayerStates.PlayerInRaft:
                        SetVendorType(EVendorType.PlayerInventory);
                        break;
                    case EPlayerStates.PlayerInWater:
                        SetVendorType(EVendorType.None);
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
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        [Inject]
        private void Construct(IGameObservable<ResourceItem> observable, IResourceService resourceService, IPlayerService playerService)
        {
            _observableItem = observable;
            _resourceService = resourceService;
            _playerService = playerService;
            _playerUpgradeSettings = playerService.UpgradeSettings;
            _playerUpgradeService = playerService.UpgradeService;
        }

        public void SetVendorType(EVendorType vendorType)
        {
            if (vendorType == EVendorType.None)
            {
                if (State == EPlayerStates.PlayerInRaft)
                {
                    vendorType = EVendorType.PlayerInventory;
                }
            }
            
            //todo: говно какое-то
            switch (vendorType)
            {
                case EVendorType.None:
                    aqualangUpgradeElement.SetActiveWindow(false);
                    pedestalWeapon.SetActiveWindow(false);
                    playerUpgrade.SetActiveWindow(false);
                    weaponPanel.SetActiveWindow(false);
                    _weaponInventory.SetActiveWindow(false);
                    break;
                case EVendorType.WeaponUpgrade:
                    aqualangUpgradeElement.SetActiveWindow(false);
                    pedestalWeapon.SetActiveWindow(false);
                    playerUpgrade.SetActiveWindow(false);
                    weaponPanel.SetActiveWindow(true);
                    _weaponInventory.SetActiveWindow(false);
                    break;
                case EVendorType.PlayerUpgrade:
                    aqualangUpgradeElement.SetActiveWindow(false);
                    pedestalWeapon.SetActiveWindow(false);
                    weaponPanel.SetActiveWindow(false);
                    playerUpgrade.SetActiveWindow(true);
                    _weaponInventory.SetActiveWindow(false);
                    break;
                case EVendorType.PedestalUpgrade:
                    aqualangUpgradeElement.SetActiveWindow(false);
                    pedestalWeapon.SetActiveWindow(true);
                    weaponPanel.SetActiveWindow(false);
                    playerUpgrade.SetActiveWindow(false);
                    _weaponInventory.SetActiveWindow(false);
                    break;
                case EVendorType.PlayerInventory:
                    aqualangUpgradeElement.SetActiveWindow(false);
                    pedestalWeapon.SetActiveWindow(false);
                    weaponPanel.SetActiveWindow(false);
                    playerUpgrade.SetActiveWindow(false);
                    _weaponInventory.SetActiveWindow(true);
                    break;
                case EVendorType.AqualangUpgrade:
                    aqualangUpgradeElement.SetActiveWindow(true);
                    pedestalWeapon.SetActiveWindow(false);
                    playerUpgrade.SetActiveWindow(false);
                    weaponPanel.SetActiveWindow(false);
                    _weaponInventory.SetActiveWindow(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(vendorType), vendorType, null);
            }
        }

        private void Start()
        {
            
            playerUpgrade.Initialize(_resourceService, _playerUpgradeSettings, _playerUpgradeService);
            aqualangUpgradeElement.Initialize(_resourceService, _playerUpgradeSettings, _playerUpgradeService, _observableItem);
            _observableItem.AddObserver(playerUpgrade);
            _observableItem.AddObserver(aqualangUpgradeElement);
            _playerService.AddListener(OnChangePlayerState);
            SetVendorType(EVendorType.PlayerInventory);
        }

        private void OnChangePlayerState(EPlayerStates state, EntityPlayer player)
        {
            State = state;
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        private void OnDestroy()
        {
            if (_playerService != null)
            {
                _playerService.RemoveListener(OnChangePlayerState);
            }
            if (_observableItem != null)
            {
                _observableItem.RemoveObserver(playerUpgrade);
            }
        }
    }
}