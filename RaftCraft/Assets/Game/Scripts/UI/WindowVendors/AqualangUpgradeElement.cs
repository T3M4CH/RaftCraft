using System;
using DG.Tweening;
using Game.Scripts.Core.Interface;
using Game.Scripts.Player.HeroPumping.Enums;
using Game.Scripts.Player.HeroPumping.Interfaces;
using Game.Scripts.ResourceController;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ResourceController.Interfaces;
using Game.Scripts.UI.WindowVendors.Structs;
using GTapSoundManager.SoundManager;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.WindowVendors
{
    public class AqualangUpgradeElement : MonoBehaviour, IGameObserver<ResourceItem>
    {
        [SerializeField] private SoundAsset soundAsset;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Sprite activeButton;
        [SerializeField] private Sprite inactiveButton;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private RectTransform moneyRect;
        [SerializeField] private RawImage _renderTexture;
        [SerializeField] private Image _backMaskFish;

        
        [SerializeField, FoldoutGroup("Upgrade")] private RectTransform gridHolder;
        [SerializeField, FoldoutGroup("Upgrade")] private ScubaUpgradeElement _scubaUpgradeElementPrefab;
        [SerializeField, FoldoutGroup("Upgrade")] private IconConfig _iconConfig;
        
        private bool _isBlock;
        private bool _initialize;
        private IResourceService _resourceService;
        private IPlayerUpgradeService _upgradeService;
        private IPlayerUpgradeSettings _upgradeSettings;
        private IGameObservable<ResourceItem> _observable;

        public void Initialize(IResourceService resourceService, IPlayerUpgradeSettings upgradeSettings, IPlayerUpgradeService upgradeService, IGameObservable<ResourceItem> observable)
        {
            if (_initialize) return;
            _initialize = true;
            _observable = observable;

            _upgradeService = upgradeService;
            _upgradeSettings = upgradeSettings;

            _resourceService = resourceService;

            upgradeButton.onClick.AddListener(Upgrade);
            InitButtons();
        }
        
        private void InitButtons()
        {
            var upgradeParams = new[]
            {
                (EPlayerUpgradeType.FishLevel, _iconConfig.FishLevel),
                (EPlayerUpgradeType.WaterSpeed, _iconConfig.WaterSpeedIcon),
                (EPlayerUpgradeType.Oxygen, _iconConfig.OxygenIcon),
                (EPlayerUpgradeType.MaxDepth, _iconConfig.MaxDepthIcon),
            };

            for (var i = 0; i < upgradeParams.Length; i++)
            {
                var param = upgradeParams[i];

                var element = Instantiate(_scubaUpgradeElementPrefab, gridHolder);
                element.Initialize(param.Item1, param.Item2, _upgradeSettings, _resourceService, _upgradeService);
                _observable.AddObserver(element);
            }
        }

        public void SetActiveRenderTexture(bool value)
        {
            _renderTexture.gameObject.SetActive(value);
            
            _backMaskFish.DOFade(value? 0.4470588f : 0f, 0.5f);
        }
        
        public void SetActiveWindow(bool value)
        {
            if (_isBlock) return;
            rectTransform.DOKill();

            rectTransform.DOScale(value ? Vector3.one : Vector3.zero, 0.2f);
        }

        public void PerformNotify(ResourceItem data)
        {
            if (!_initialize) return;

            if (data.Type == EResourceType.Wood)
            {
                var currentPrice = _upgradeService.GetPrice(EPlayerUpgradeType.Oxygen);

                var level = _upgradeSettings.GetLevel(EPlayerUpgradeType.Oxygen);

                if (level >= 11)
                {
                    SetActiveWindow(false);
                    _isBlock = true;
                }
                
                if (data.Count >= currentPrice)
                {
                    upgradeButton.image.sprite = activeButton;
                }
                else
                {
                    upgradeButton.image.sprite = inactiveButton;
                }

                levelText.text = $"LVL {level}";
                priceText.text = currentPrice.ToString();
                
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
                LayoutRebuilder.ForceRebuildLayoutImmediate(moneyRect);
                Canvas.ForceUpdateCanvases();
            }
        }

        private void Upgrade()
        {
            if (_resourceService.TryRemove(EResourceType.Wood, _upgradeService.GetPrice(EPlayerUpgradeType.Oxygen)))
            {
                soundAsset.Play();
                _upgradeService.Upgrade(EPlayerUpgradeType.FishLevel, EPlayerUpgradeType.MaxDepth, EPlayerUpgradeType.WaterSpeed, EPlayerUpgradeType.Oxygen);
                _resourceService.TryRemove(EResourceType.Wood, 0);
            }
        }

        private void OnDestroy()
        {
            _backMaskFish.DOKill();
        }
    }
}