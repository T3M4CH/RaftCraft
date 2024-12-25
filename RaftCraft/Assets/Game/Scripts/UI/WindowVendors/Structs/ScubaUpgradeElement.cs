using System;
using Game.Scripts.Core.Interface;
using Game.Scripts.Player.HeroPumping.Enums;
using Game.Scripts.Player.HeroPumping.Interfaces;
using Game.Scripts.ResourceController;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ResourceController.Interfaces;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Game.Scripts.UI.WindowVendors.Structs
{
    public class ScubaUpgradeElement : MonoBehaviour, IGameObserver<ResourceItem>
    {
        [SerializeField] public TMP_Text textInfo;
        [SerializeField] public TMP_Text upgradeInfo;
        [SerializeField] private RectTransform _rectPrice;
        [SerializeField] private TextMeshProUGUI _textCost;
        [SerializeField] private Button _buttonUpgrade;
        [SerializeField] private Image iconImage;

        private EPlayerUpgradeType _currentType;
        private IPlayerUpgradeSettings _upgradeSettings;
        private IResourceService _resourceService;
        private IPlayerUpgradeService _upgradeService;
        public void Initialize(EPlayerUpgradeType upgradeType, Sprite sprite, IPlayerUpgradeSettings upgradeSettings, IResourceService resourceService, IPlayerUpgradeService upgradeService)
        {
            iconImage.sprite = sprite;
            
            _currentType = upgradeType;
            _upgradeSettings = upgradeSettings;
            _resourceService = resourceService;
            _upgradeService = upgradeService;

            textInfo.text = upgradeType.ToString();
            _upgradeSettings.OnUpgrade += ValidateElement;
            
            ValidateElement(upgradeType);
        }

        private void Start()
        {
            _buttonUpgrade.onClick.AddListener(BuyUpgrade);
            ValidateButton();
        }

        private void ValidateButton()
        {
            if (_upgradeService.HaveMax(_currentType))
            {
                _buttonUpgrade.interactable = false;
                upgradeInfo.text = "MAX";
                _rectPrice.gameObject.SetActive(false);
                return;
            }

            _textCost.text = $"{_upgradeService.GetPrice(_currentType)}";
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectPrice);
            Canvas.ForceUpdateCanvases();
            _buttonUpgrade.interactable =
                _resourceService.HaveCount(EResourceType.Wood, _upgradeService.GetPrice(_currentType));
        }

        private void OnDestroy()
        {
            _buttonUpgrade.onClick.RemoveListener(BuyUpgrade);
        }

        private void BuyUpgrade()
        {
            if (_upgradeService.IsMaxLevelAqua)
            {
                return;
            }

            if (_resourceService.TryRemove(EResourceType.Wood, _upgradeService.GetPrice(_currentType)))
            {
                _upgradeService.Upgrade(_currentType);
                ValidateButton();
            }
        }

        private void ValidateElement(EPlayerUpgradeType playerUpgradeType)
        {
            if (playerUpgradeType != _currentType) return;

            var currentValue = _upgradeSettings.GetValue<int>(playerUpgradeType);
            var nextValue = 0;
            
            try
            {
                var nextLevel = _upgradeSettings.GetLevel(playerUpgradeType) + 1;
                nextValue = _upgradeSettings.GetValue<int>(playerUpgradeType, nextLevel);
            }
            catch (Exception e)
            {
                upgradeInfo.text = currentValue.ToString();
                return;
            }
            
            upgradeInfo.text = $"LVL {_upgradeSettings.GetLevel(_currentType)}";
            ValidateButton();
        }

        public void PerformNotify(ResourceItem data)
        {
            ValidateButton();
        }
    }
}