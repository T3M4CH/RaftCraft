using System;
using Game.Scripts.Core.Interface;
using Game.Scripts.Player.WeaponController;
using Game.Scripts.Player.WeaponController.WeaponsData;
using Game.Scripts.ResourceController;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ResourceController.Interfaces;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Elements.WeaponSystem
{
    public class ButtonUpgradeWeapon : MonoBehaviour, IGameObserver<WeaponUpgrade>, IGameObserver<ResourceItem>
    {
        public event Action<WeaponId> OnClickButton;

        [SerializeField, FoldoutGroup("UI")] private Button _button;
        [SerializeField, FoldoutGroup("UI")] private Image _imageButton;
        [SerializeField, FoldoutGroup("UI")] private TextMeshProUGUI _textLevel;
        [SerializeField, FoldoutGroup("UI")] private Image _imageIcon;
        [SerializeField, FoldoutGroup("UI")] private Image _imageResource;
        [SerializeField, FoldoutGroup("UI")] private TextMeshProUGUI _textCost;
        [SerializeField, FoldoutGroup("UI")] private RectTransform _rectContentPrice;
        [SerializeField, FoldoutGroup("UI Settings")] private Sprite _spriteUnLock;
        [SerializeField, FoldoutGroup("UI Settings")] private Sprite _spriteLock;
        [SerializeField, FoldoutGroup("UI Settings")] private Sprite _spriteUnLockResource;
        [SerializeField, FoldoutGroup("UI Settings")] private Sprite _spriteLockResource;
        
        [SerializeField, FoldoutGroup("Settings")] private IconConfig _config; 

        private WeaponId _weaponId;
        private WeaponsDataSettings _data;
        private int _levelWeapon;
        private IResourceService _resourceService;
        
        public void Initialize(WeaponId id, WeaponsDataSettings data, IResourceService resourceService)
        {
            _data = data;
            _weaponId = id;
            _resourceService = resourceService;
            _imageIcon.sprite = _config.GetIconWeapon(id);
        }
        
        public void PerformNotify(WeaponUpgrade data)
        {
            if (_weaponId != data.Id)
            {
                return;
            }
            Debug.Log($"State weapon: {data.Id}:{data.UnLock}");
            gameObject.SetActive(data.UnLock);
            _levelWeapon = data.Level + 1;
            _textLevel.text = $"LVL {data.Level + 1}";
            _textCost.text = $"{_data.Cost(data.Level + 1)}";
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectContentPrice);
            Canvas.ForceUpdateCanvases();
        }
        
        public void PerformNotify(ResourceItem data)
        {
            if (data.Type != EResourceType.CoinGold)
            {
                return;
            }
            
            UpdateStateButton(data.Count);
        }

        private void UpdateStateButton(int money)
        {
            _imageButton.sprite = HaveMoney(money) ? _spriteUnLock : _spriteLock;
            _imageResource.sprite = HaveMoney(money) ? _spriteUnLockResource : _spriteLockResource;
            //_button.interactable = HaveMoney(money);
        }

        private bool HaveMoney(int money)
        {
            return money >= _data.Cost(_levelWeapon);
        }

        public void Click()
        {
            if (_resourceService.TryRemove(EResourceType.CoinGold, _data.Cost(_levelWeapon)))
            {
                OnClickButton?.Invoke(_weaponId);
                UpdateStateButton(_resourceService.GetValue(EResourceType.CoinGold));
            }
        }

        
    }
}
