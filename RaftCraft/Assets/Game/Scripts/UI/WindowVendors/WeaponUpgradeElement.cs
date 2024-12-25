using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Scripts.Core.Interface;
using Game.Scripts.Player.WeaponController;
using Game.Scripts.Player.WeaponController.Interface;
using Game.Scripts.Player.WeaponController.WeaponsData;
using Game.Scripts.Player.WeaponController.WeaponShop;
using Game.Scripts.ResourceController;
using Game.Scripts.ResourceController.Interfaces;
using Game.Scripts.UI.Elements.WeaponSystem;
using GTapSoundManager.SoundManager;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.UI.WindowVendors
{
    public class WeaponUpgradeElement : MonoBehaviour
    {
        [SerializeField] private SoundAsset soundAsset;
        [SerializeField, FoldoutGroup("Settings")] private WeaponsDataSettings _dataPistol;
        [SerializeField, FoldoutGroup("Settings")] private WeaponsDataSettings _dataRifle;
        [SerializeField, FoldoutGroup("Settings")] private WeaponsDataSettings _dataShotGun;

        [SerializeField, FoldoutGroup("UI")] private RectTransform _panel;
        [SerializeField, FoldoutGroup("UI")] private RectTransform _content;
        [SerializeField, FoldoutGroup("Settings")] private ButtonUpgradeWeapon _prefabButton;

        private List<ButtonUpgradeWeapon> _cells = new List<ButtonUpgradeWeapon>();
        private IGameObservable<WeaponUpgrade> _observable;
        private IWeaponUpgradeService _upgradeService;
        private IResourceService _resourceService;
        private IGameObservable<ResourceItem> _observableResource;
        private IShopView _shopView;
        
        [Inject]
        private void Init(IGameObservable<WeaponUpgrade> observable, IGameObservable<ResourceItem> observableResource,IResourceService resourceService, IWeaponUpgradeService upgradeService, IShopView shopView)
        {
            _upgradeService = upgradeService;
            _observable = observable;
            _resourceService = resourceService;
            _observableResource = observableResource;
            _shopView = shopView;
        }

        private void Start()
        {
            CreateCell(_dataPistol);
            CreateCell(_dataShotGun);
            CreateCell(_dataRifle);
        }
        
        public void SetActiveWindow(bool value)
        {
            _panel.DOKill();

            _panel.DOScale(value ? Vector3.one : Vector3.zero, 0.2f);
        }

        private ButtonUpgradeWeapon CreateCell(WeaponsDataSettings settings)
        {
            var cell = Instantiate(_prefabButton.gameObject, _content).GetComponent<ButtonUpgradeWeapon>();
            cell.Initialize(settings.CurrentId, settings, _resourceService);
            cell.OnClickButton += OnClickButton;
            _cells.Add(cell);
            _observable.AddObserver(cell);
            _observableResource.AddObserver(cell);
            return cell;
        }

        private void OnClickButton(WeaponId id)
        {
            soundAsset.Play();
            _upgradeService.Upgrade(id);
        }

        private void OnDestroy()
        {
            foreach (var cell in _cells)
            {
                cell.OnClickButton -= OnClickButton;
                _observable.RemoveObserver(cell);
                _observableResource.RemoveObserver(cell);
            }
        }
    }
}
