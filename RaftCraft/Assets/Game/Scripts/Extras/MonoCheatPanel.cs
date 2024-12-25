using System;
using Game.Scripts.Core.Interface;
using Game.Scripts.Days;
using Game.Scripts.Joystick.Extras;
using Game.Scripts.Player.WeaponController.WeaponShop;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ResourceController.Interfaces;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonoCheatPanel : MonoBehaviour, IGameObserver<WeaponPrice>
{
    [SerializeField] private Button crossButton;
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text weaponUpgrade;

    [SerializeField, FoldoutGroup("ResourcesAndDays")] private GameObject resourcesWindow;
    [SerializeField, FoldoutGroup("ResourcesAndDays")] private Button resourceButtonPanel;
    [SerializeField, FoldoutGroup("ResourcesAndDays")] private Button typeButton;
    [SerializeField, FoldoutGroup("ResourcesAndDays")] private Button dayButton;
    [SerializeField, FoldoutGroup("ResourcesAndDays")] private TMP_Text typeText;
    [SerializeField, FoldoutGroup("ResourcesAndDays")] private Button invokeButton;
    [SerializeField, FoldoutGroup("ResourcesAndDays")] private TMP_InputField inputField;

    [SerializeField, FoldoutGroup("Upgrades_A")] private Button upgradeButtonPanel;
    [SerializeField, FoldoutGroup("Upgrades_A")] private GameObject upgradesWindow;

    [SerializeField, FoldoutGroup("Upgrades_P")] private Button upgradePlayerButtonPanel;
    [SerializeField, FoldoutGroup("Upgrades_P")] private GameObject upgradesPlayerWindow;

    private int _currentTouchCount;
    private float _currentLeftTime;
    private int _currentResourceId;
    private bool _resourceSelected;
    private IDayService _dayController;
    private EResourceType[] _resourceTypes;
    private IGameResourceService _resourceService;
    private PlayerInput.PlayerActions _playerInput;

    private const float Delay = 1f;
    private const int TargetTouchCount = 4;

    [Inject]
    private void Construct(InputSingleton inputSingleton, IDayService dayController, IGameResourceService resourceService, IGameObservable<WeaponPrice> observable)
    {
        _dayController = dayController;
        _resourceService = resourceService;
        _playerInput = inputSingleton.Instance.Player;

        observable.AddObserver(this);

#if !UNITY_EDITOR
        Destroy(gameObject);
#endif
    }

    private void Update()
    {
        if (_playerInput.Touch.WasReleasedThisFrame())
        {
            _currentTouchCount += 1;

            switch (_currentTouchCount)
            {
                case 1:
                    _currentLeftTime = Delay;
                    break;
                case TargetTouchCount:
                    panel.SetActive(true);
                    _currentTouchCount = 0;
                    break;
            }
        }

        _currentLeftTime -= Time.deltaTime;

        if (_currentLeftTime < 0)
        {
            _currentTouchCount = 0;
        }
    }

    private void Start()
    {
        dayButton.onClick.AddListener(ChangeDayType);
        crossButton.onClick.AddListener(CloseWindow);
        typeButton.onClick.AddListener(ChangeResourceType);
        invokeButton.onClick.AddListener(InvokeResourceDay);
        upgradeButtonPanel.onClick.AddListener(OpenUpgradesWindow);
        resourceButtonPanel.onClick.AddListener(OpenResourcesWindow);
        upgradePlayerButtonPanel.onClick.AddListener(() => upgradesPlayerWindow.SetActive(true));
        _resourceTypes = (EResourceType[])Enum.GetValues(typeof(EResourceType));

        ChangeResourceType();
    }

    private void OpenResourcesWindow()
    {
        resourcesWindow.SetActive(true);
    }

    private void OpenUpgradesWindow()
    {
        upgradesWindow.SetActive(true);
    }

    private void CloseWindow()
    {
        if (resourcesWindow.activeSelf || upgradesWindow.activeSelf || upgradesPlayerWindow.activeSelf)
        {
            resourcesWindow.SetActive(false);
            upgradesWindow.SetActive(false);
            upgradesPlayerWindow.SetActive(false);
        }
        else
        {
            panel.SetActive(false);
        }
    }

    private void ChangeDayType()
    {
        var dayButtonImage = dayButton.image;
        var color = dayButtonImage.color;
        color.a = 1f;
        dayButtonImage.color = color;

        var typeButtonImage = typeButton.image;
        color = typeButtonImage.color;
        color.a = 0.5f;
        typeButtonImage.color = color;

        _resourceSelected = false;
    }

    private void ChangeResourceType()
    {
        var dayButtonImage = dayButton.image;
        var color = dayButtonImage.color;
        color.a = 0.5f;
        dayButtonImage.color = color;

        var typeButtonImage = typeButton.image;
        color = typeButtonImage.color;
        color.a = 1f;
        typeButtonImage.color = color;

        if (!_resourceSelected)
        {
            _resourceSelected = true;
            return;
        }

        _currentResourceId += 1;
        if (_currentResourceId > _resourceTypes.Length - 1)
        {
            _currentResourceId = 0;
        }

        typeText.text = _resourceTypes[_currentResourceId].ToString();
    }

    private void InvokeResourceDay()
    {
        if (!int.TryParse(inputField.text, out var value))
        {
            value = 0;
        }

        if (_resourceSelected)
        {
            _resourceService.Add(_resourceTypes[_currentResourceId], value);
        }
        else
        {
            _dayController.SetDay(value);
        }
    }

    public void PerformNotify(WeaponPrice data)
    {
        weaponUpgrade.text = data.Id.ToString();
    }
}