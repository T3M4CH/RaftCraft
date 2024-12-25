using System;
using DG.Tweening;
using Game.Scripts.Core.Interface;
using Game.Scripts.Player.WeaponController.WeaponShop;
using Game.Scripts.Player.WeaponController.WeaponShop.ViewUI;
using Game.Scripts.ResourceController;
using Newtonsoft.Json;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.UI.WindowVendors
{
    public class PedestalWeaponElement : MonoBehaviour, IGameObserver<WeaponPrice>, IGameObserver<ResourceItem>
    {
        [SerializeField] private RectTransform rectTransform;

        [SerializeField, FoldoutGroup("UI")] private RectTransform _content;

        [SerializeField, FoldoutGroup("Settings")]
        private CellWeaponBuy _prefabCell;

        private WeaponPrice _currentPrice;
        private CellWeaponBuy _current;
        
        
        private IShopView _shopView;
        private IWeaponShop _shop;
        private IGameObservable<WeaponPrice> _gameObservable;
        private IGameObservable<ResourceItem> _itemObserver;
        [Inject]
        private void Construct(IShopView shopView, IWeaponShop shop, IGameObservable<WeaponPrice> gameObservable, IGameObservable<ResourceItem> itemObserver)
        {
            _shopView = shopView;
            _shop = shop;
            _gameObservable = gameObservable;
            _itemObserver = itemObserver;
            _gameObservable.AddObserver(this);
            _itemObserver.AddObserver(this);
        }
        
        public void SetActiveWindow(bool value)
        {
            rectTransform.DOKill();

            rectTransform.DOScale(value ? Vector3.one : Vector3.zero, 0.2f);
        
        }
        
        public void PerformNotify(WeaponPrice data)
        {
            _currentPrice = data;
            if (data == null)
            {
                if (_current != null)
                {
                    _current.ButtonBuy.onClick.RemoveListener(BuyWeapon);
                    Destroy(_current.gameObject);
                    _current = null;
                    SetActiveWindow(false);
                }
                return;
            }
            if (_current == null)
            {
                _current = Instantiate(_prefabCell, _content);
                _current.ButtonBuy.onClick.AddListener(BuyWeapon);
                _current.Init(data);
            }

            if (_current.CurrentId != data.Id)
            {
                _current.ButtonBuy.onClick.RemoveListener(BuyWeapon);
                Destroy(_current.gameObject);
                _current = null;
                PerformNotify(data);
                return;
            }

            if (_current != null)
            {
                _current.UpdateStateLock(_shopView.HaveBuyCurrent());
            }
        }

        private void BuyWeapon()
        {
            if (_shop == null)
            {
                return;
            }

            _shop.TryBuyWeapon();
        }

        private void OnDestroy()
        {
            if (_gameObservable != null)
            {
                _gameObservable.RemoveObserver(this);
            }

            if (_current != null)
            {
                _current.ButtonBuy.onClick.RemoveListener(BuyWeapon);
            }

            if (_itemObserver != null)
            {
                _itemObserver.RemoveObserver(this);
            }
        }

        public void PerformNotify(ResourceItem data)
        {
            if (_currentPrice == null || _current == null)
            {
                return;
            }

            if (data.Type == _currentPrice.ItemType)
            {
                _current.UpdateStateLock(_shopView.HaveBuyCurrent());
            }
        }
    }
}
