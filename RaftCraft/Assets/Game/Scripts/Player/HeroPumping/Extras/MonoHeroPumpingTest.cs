using System.Globalization;
using Game.Scripts.Player.HeroPumping.Enums;
using Game.Scripts.Player.HeroPumping.Interfaces;
using Game.Scripts.Player.Spawners;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ResourceController.Interfaces;
using UnityEngine.InputSystem;
using Reflex.Attributes;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class MonoHeroPumpingTest : MonoBehaviour
{
    [SerializeField] private GameObject window;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button upgradeButton2;
    [SerializeField] private Button resetButton2;
    [SerializeField] private Button healthButton;
    [SerializeField] private TMP_Text _maxHealthValue;
    [SerializeField] private TMP_Text _healthLevel;
    [SerializeField] private Button raftSpeedButton;
    [SerializeField] private TMP_Text _raftSpeedValue;
    [SerializeField] private TMP_Text _raftSpeedLevel;
    [SerializeField] private Button waterSpeedButton;
    [SerializeField] private TMP_Text _waterSpeedValue;
    [SerializeField] private TMP_Text _waterSpeedLevel;
    [SerializeField] private Button capacityButton;
    [SerializeField] private TMP_Text _capacityValue;
    [SerializeField] private TMP_Text _capacitySpeedLevel;
    [SerializeField] private Button depthButton;
    [SerializeField] private TMP_Text _fishValue;
    [SerializeField] private TMP_Text _fishLevel;
    [SerializeField] private TMP_Text _depthValue;
    [SerializeField] private TMP_Text _depthLevel;
    [SerializeField] private TMP_Text _oxygenValue;
    [SerializeField] private TMP_Text _oxygenLevel;

    private IResourceService _resourceService;
    private IPlayerUpgradeSettings _playerUpgradeSettings;
    private IPlayerUpgradeService _playerUpgradeService;

    [Inject]
    private void Construct(IResourceService resourceService, IPlayerService playerService)
    {
        _resourceService = resourceService;
        _playerUpgradeService = playerService.UpgradeService;
        _playerUpgradeSettings = playerService.UpgradeSettings;
    }

    private void PerformHealthButton()
    {
        if (!_playerUpgradeService.Upgrade(EPlayerUpgradeType.MaxHealth))
        {
            healthButton.interactable = false;
        }

        _maxHealthValue.text = _playerUpgradeSettings.GetValue<float>(EPlayerUpgradeType.MaxHealth).ToString(CultureInfo.InvariantCulture);
        _healthLevel.text = _playerUpgradeSettings.GetLevel(EPlayerUpgradeType.MaxHealth).ToString();
    }

    private void PerformRaftSpeedButton()
    {
        if (!_playerUpgradeService.Upgrade(EPlayerUpgradeType.RaftSpeed))
        {
            raftSpeedButton.interactable = false;
        }

        _raftSpeedValue.text = _playerUpgradeSettings.GetValue<float>(EPlayerUpgradeType.RaftSpeed).ToString(CultureInfo.InvariantCulture);
        _raftSpeedLevel.text = _playerUpgradeSettings.GetLevel(EPlayerUpgradeType.RaftSpeed).ToString();
    }

    private void PerformWaterSpeedButton()
    {
        if (!_playerUpgradeService.Upgrade(EPlayerUpgradeType.WaterSpeed))
        {
            waterSpeedButton.interactable = false;
        }

        _waterSpeedValue.text = _playerUpgradeSettings.GetValue<float>(EPlayerUpgradeType.WaterSpeed).ToString(CultureInfo.InvariantCulture);
        _waterSpeedLevel.text = _playerUpgradeSettings.GetLevel(EPlayerUpgradeType.WaterSpeed).ToString();
    }

    private void PerformDepthButton()
    {
        if (!_playerUpgradeService.Upgrade(EPlayerUpgradeType.MaxDepth))
        {
            depthButton.interactable = false;
        }

        _depthValue.text = _playerUpgradeSettings.GetValue<float>(EPlayerUpgradeType.MaxDepth).ToString(CultureInfo.InvariantCulture);
        _depthLevel.text = _playerUpgradeSettings.GetLevel(EPlayerUpgradeType.MaxDepth).ToString();
    }

    private void PerformCapacityButton()
    {
        if (!_playerUpgradeService.Upgrade(EPlayerUpgradeType.Capacity))
        {
            capacityButton.interactable = false;
        }

        _capacityValue.text = _playerUpgradeSettings.GetValue<int>(EPlayerUpgradeType.Capacity).ToString(CultureInfo.InvariantCulture);
        _capacitySpeedLevel.text = _playerUpgradeSettings.GetLevel(EPlayerUpgradeType.Capacity).ToString();
    }

    private void PerformUpgradePlayerButton()
    {
        if(_playerUpgradeService.IsMaxLevelPlayer) return;

        _playerUpgradeService.Upgrade(EPlayerUpgradeType.MaxHealth, EPlayerUpgradeType.RaftSpeed);
        
        _raftSpeedValue.text = _playerUpgradeSettings.GetValue<float>(EPlayerUpgradeType.RaftSpeed).ToString(CultureInfo.InvariantCulture);
        _raftSpeedLevel.text = _playerUpgradeSettings.GetLevel(EPlayerUpgradeType.RaftSpeed).ToString();
        
        _maxHealthValue.text = _playerUpgradeSettings.GetValue<float>(EPlayerUpgradeType.MaxHealth).ToString(CultureInfo.InvariantCulture);
        _healthLevel.text = _playerUpgradeSettings.GetLevel(EPlayerUpgradeType.MaxHealth).ToString();
    }

    private void ResetPlayerUpgrades()
    {
        _playerUpgradeService.Upgrade(EPlayerUpgradeType.RaftSpeed, 1);
        _playerUpgradeService.Upgrade(EPlayerUpgradeType.MaxHealth, 1);
        
        _raftSpeedValue.text = _playerUpgradeSettings.GetValue<float>(EPlayerUpgradeType.RaftSpeed).ToString(CultureInfo.InvariantCulture);
        _raftSpeedLevel.text = _playerUpgradeSettings.GetLevel(EPlayerUpgradeType.RaftSpeed).ToString();
        
        _maxHealthValue.text = _playerUpgradeSettings.GetValue<float>(EPlayerUpgradeType.MaxHealth).ToString(CultureInfo.InvariantCulture);
        _healthLevel.text = _playerUpgradeSettings.GetLevel(EPlayerUpgradeType.MaxHealth).ToString();
    }

    private void PerformUpgradeButton()
    {
        if (_playerUpgradeService.IsMaxLevelAqua) return;

        _playerUpgradeService.Upgrade(EPlayerUpgradeType.FishLevel, EPlayerUpgradeType.MaxDepth, EPlayerUpgradeType.WaterSpeed, EPlayerUpgradeType.Oxygen);

        _fishValue.text = _playerUpgradeSettings.GetValue<int>(EPlayerUpgradeType.FishLevel).ToString();
        _fishLevel.text = _playerUpgradeSettings.GetLevel(EPlayerUpgradeType.FishLevel).ToString();

        _depthValue.text = _playerUpgradeSettings.GetValue<float>(EPlayerUpgradeType.MaxDepth).ToString(CultureInfo.InvariantCulture);
        _depthLevel.text = _playerUpgradeSettings.GetLevel(EPlayerUpgradeType.MaxDepth).ToString();

        _waterSpeedValue.text = _playerUpgradeSettings.GetValue<float>(EPlayerUpgradeType.WaterSpeed).ToString(CultureInfo.InvariantCulture);
        _waterSpeedLevel.text = _playerUpgradeSettings.GetLevel(EPlayerUpgradeType.WaterSpeed).ToString();
        
        _oxygenValue.text = _playerUpgradeSettings.GetValue<float>(EPlayerUpgradeType.Oxygen).ToString(CultureInfo.InvariantCulture);
        _oxygenLevel.text = _playerUpgradeSettings.GetLevel(EPlayerUpgradeType.Oxygen).ToString();
    }

    private void ResetUpgrades()
    {
        _playerUpgradeService.Upgrade(EPlayerUpgradeType.WaterSpeed, 1);
        _playerUpgradeService.Upgrade(EPlayerUpgradeType.FishLevel, 1);
        _playerUpgradeService.Upgrade(EPlayerUpgradeType.MaxDepth, 1);
        _playerUpgradeService.Upgrade(EPlayerUpgradeType.Oxygen, 1);

        _fishValue.text = _playerUpgradeSettings.GetValue<int>(EPlayerUpgradeType.FishLevel).ToString();
        _fishLevel.text = _playerUpgradeSettings.GetLevel(EPlayerUpgradeType.FishLevel).ToString();

        _depthValue.text = _playerUpgradeSettings.GetValue<float>(EPlayerUpgradeType.MaxDepth).ToString(CultureInfo.InvariantCulture);
        _depthLevel.text = _playerUpgradeSettings.GetLevel(EPlayerUpgradeType.MaxDepth).ToString();

        _waterSpeedValue.text = _playerUpgradeSettings.GetValue<float>(EPlayerUpgradeType.WaterSpeed).ToString(CultureInfo.InvariantCulture);
        _waterSpeedLevel.text = _playerUpgradeSettings.GetLevel(EPlayerUpgradeType.WaterSpeed).ToString();
        
        _oxygenValue.text = _playerUpgradeSettings.GetValue<float>(EPlayerUpgradeType.Oxygen).ToString(CultureInfo.InvariantCulture);
        _oxygenLevel.text = _playerUpgradeSettings.GetLevel(EPlayerUpgradeType.Oxygen).ToString();
    }

    private void Start()
    {
        resetButton.onClick.AddListener(ResetUpgrades);
        upgradeButton.onClick.AddListener(PerformUpgradeButton);
        healthButton.onClick.AddListener(PerformHealthButton);
        waterSpeedButton.onClick.AddListener(PerformWaterSpeedButton);
        raftSpeedButton.onClick.AddListener(PerformRaftSpeedButton);
        depthButton.onClick.AddListener(PerformDepthButton);
        capacityButton.onClick.AddListener(PerformCapacityButton);
        upgradeButton2.onClick.AddListener(PerformUpgradePlayerButton);
        resetButton2.onClick.AddListener(ResetPlayerUpgrades);

        _fishValue.text = _playerUpgradeSettings.GetValue<int>(EPlayerUpgradeType.FishLevel).ToString();
        _fishLevel.text = _playerUpgradeSettings.GetLevel(EPlayerUpgradeType.FishLevel).ToString();

        _depthValue.text = _playerUpgradeSettings.GetValue<float>(EPlayerUpgradeType.MaxDepth).ToString(CultureInfo.InvariantCulture);
        _depthLevel.text = _playerUpgradeSettings.GetLevel(EPlayerUpgradeType.MaxDepth).ToString();

        _waterSpeedValue.text = _playerUpgradeSettings.GetValue<float>(EPlayerUpgradeType.WaterSpeed).ToString(CultureInfo.InvariantCulture);
        _waterSpeedLevel.text = _playerUpgradeSettings.GetLevel(EPlayerUpgradeType.WaterSpeed).ToString();

        _oxygenValue.text = _playerUpgradeSettings.GetValue<float>(EPlayerUpgradeType.Oxygen).ToString(CultureInfo.InvariantCulture);
        _oxygenLevel.text = _playerUpgradeSettings.GetLevel(EPlayerUpgradeType.Oxygen).ToString();
        
        _raftSpeedValue.text = _playerUpgradeSettings.GetValue<float>(EPlayerUpgradeType.RaftSpeed).ToString(CultureInfo.InvariantCulture);
        _raftSpeedLevel.text = _playerUpgradeSettings.GetLevel(EPlayerUpgradeType.RaftSpeed).ToString();
        
        _maxHealthValue.text = _playerUpgradeSettings.GetValue<float>(EPlayerUpgradeType.MaxHealth).ToString(CultureInfo.InvariantCulture);
        _healthLevel.text = _playerUpgradeSettings.GetLevel(EPlayerUpgradeType.MaxHealth).ToString();
    }
}