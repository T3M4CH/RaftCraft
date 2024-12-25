using DG.Tweening;
using Game.Scripts.Core.Interface;
using Game.Scripts.Player.HeroPumping.Enums;
using Game.Scripts.Player.HeroPumping.Interfaces;
using Game.Scripts.ResourceController;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ResourceController.Interfaces;
using GTapSoundManager.SoundManager;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PlayerUpgradeElement : MonoBehaviour, IGameObserver<ResourceItem>
{
    [SerializeField] private SoundAsset _soundAsset;
    [SerializeField] private Sprite coinInactive;
    [SerializeField] private Sprite coinActive;
    [SerializeField] private Sprite buttonInactive;
    [SerializeField] private RectTransform rectTransform;

    [SerializeField, FoldoutGroup("Run")] private RectTransform runRect;
    [SerializeField, FoldoutGroup("Run")] private Button runButton;
    [SerializeField, FoldoutGroup("Run")] private Image coinRunImage;
    [SerializeField, FoldoutGroup("Run")] private Image runButtonSprite;
    [SerializeField, FoldoutGroup("Run")] private Sprite runActive;
    [SerializeField, FoldoutGroup("Run")] private TMP_Text runLevel;
    [SerializeField, FoldoutGroup("Run")] private TMP_Text runPrice;

    [SerializeField, FoldoutGroup("Health")] private RectTransform healthRect;
    [SerializeField, FoldoutGroup("Health")] private Button healthButton;
    [SerializeField, FoldoutGroup("Health")] private Image coinHealthImage;
    [SerializeField, FoldoutGroup("Health")] private Image healthButtonSprite;
    [SerializeField, FoldoutGroup("Health")] private Sprite healthButtonActive;
    [SerializeField, FoldoutGroup("Health")] private TMP_Text healthLevel;
    [SerializeField, FoldoutGroup("Health")] private TMP_Text healthPrice;

    private IResourceService _resourceService;
    private IPlayerUpgradeSettings _upgradeSettings;
    private IPlayerUpgradeService _upgradeService;

    private bool _initialize;
    private bool _isBlock;

    public void Initialize(IResourceService resourceService, IPlayerUpgradeSettings upgradeSettings, IPlayerUpgradeService upgradeService)
    {
        if (_initialize) return;
        _initialize = true;
        healthButton.onClick.AddListener(UpgradeHealth);
        runButton.onClick.AddListener(UpgradeSpeed);

        _upgradeService = upgradeService;
        _upgradeSettings = upgradeSettings;

        _resourceService = resourceService;
    }

    public void SetActiveWindow(bool value)
    {
        if (_isBlock) return;
        rectTransform.DOKill();

        rectTransform.DOScale(value ? Vector3.one : Vector3.zero, 0.2f);
    }

    private void UpgradeHealth()
    {
        if (_resourceService.TryRemove(EResourceType.CoinGold, _upgradeService.GetPrice(EPlayerUpgradeType.MaxHealth)))
        {
            _soundAsset.Play();
            _upgradeService.Upgrade(EPlayerUpgradeType.MaxHealth);
            _resourceService.TryRemove(EResourceType.CoinGold, 0);
        }
    }

    private void UpgradeSpeed()
    {
        if (_resourceService.TryRemove(EResourceType.CoinGold, _upgradeService.GetPrice(EPlayerUpgradeType.RaftSpeed)))
        {
            _soundAsset.Play();
            _upgradeService.Upgrade(EPlayerUpgradeType.RaftSpeed);
            _resourceService.TryRemove(EResourceType.CoinGold, 0);
        }
    }

    public void OnDestroy()
    {
        runButton.onClick.RemoveAllListeners();
        healthButton.onClick.RemoveAllListeners();
    }

    public void PerformNotify(ResourceItem data)
    {
        if (!_initialize) return;

        if (data.Type == EResourceType.CoinGold)
        {
            var currentCoins = data.Count;

            var raftLevel = _upgradeSettings.GetLevel(EPlayerUpgradeType.RaftSpeed);
            var raftPrice = _upgradeService.GetPrice(EPlayerUpgradeType.RaftSpeed);

            runLevel.text = $"LVL {raftLevel}";
            runPrice.text = raftPrice.ToString();
            if (currentCoins >= raftPrice)
            {
                runButtonSprite.sprite = runActive;
                coinRunImage.sprite = coinActive;
            }
            else
            {
                runButtonSprite.sprite = buttonInactive;
                coinRunImage.sprite = coinInactive;
            }

            if (raftLevel >= 11)
            {
                runButton.gameObject.SetActive(false);
            }

            var hLevel = _upgradeSettings.GetLevel(EPlayerUpgradeType.MaxHealth);
            var hPrice = _upgradeService.GetPrice(EPlayerUpgradeType.MaxHealth);

            healthLevel.text = $"LVL {hLevel}";
            healthPrice.text = hPrice.ToString();

            if (currentCoins >= hPrice)
            {
                healthButtonSprite.sprite = healthButtonActive;
                coinHealthImage.sprite = coinActive;
            }
            else
            {
                healthButtonSprite.sprite = buttonInactive;
                coinHealthImage.sprite = coinInactive;
            }

            if (hLevel >= 11)
            {
                healthButton.gameObject.SetActive(false);
            }

            if (hLevel >= 11 && raftLevel >= 11)
            {
                SetActiveWindow(false);
                _isBlock = true;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(runRect);
            LayoutRebuilder.ForceRebuildLayoutImmediate(healthRect);
            Canvas.ForceUpdateCanvases();
        }
    }
}