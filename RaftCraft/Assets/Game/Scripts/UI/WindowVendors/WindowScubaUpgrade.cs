using System;
using DG.Tweening;
using Game.Scripts.ResourceController.Interfaces;
using Game.Scripts.UI.WindowVendors.Structs;
using Game.Scripts.Player.HeroPumping.Enums;
using Game.Scripts.ResourceController;
using Game.Scripts.UI.WindowManager;
using Game.Scripts.Core.Interface;
using Game.Scripts.Player.HeroPumping.Interfaces;
using Game.Scripts.Player.Spawners;
using Game.Scripts.ResourceController.Enums;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindowScubaUpgrade : UIWindow, IGameObserver<ResourceItem>
{
    public event Action OnUpgrade = () => { };
    
    [SerializeField] private RectTransform gridHolder;
    [SerializeField] private ScubaUpgradeElement _scubaUpgradeElementPrefab;
    [SerializeField] private IconConfig _iconConfig;
    [SerializeField] private float _durationScaleWindow = 1f;
    [SerializeField] private Transform _elements;
    [SerializeField] private Image _background;
    
    [field:SerializeField] public Button ButtonCloseFishHelper { get; private set; }
    //[field:SerializeField] public Button UpgradeButton { get; private set; }

    private IResourceService _resourceService;
    private IPlayerUpgradeSettings _upgradeSettings;
    private IPlayerUpgradeService _upgradeService;
    private IGameObservable<ResourceItem> _observable;
    private Sequence _sequenceElements;


    [Inject]
    private void Construct(IGameObservable<ResourceItem> observable, IResourceService resourceService, IPlayerService playerService)
    {
        _resourceService = resourceService;
        _upgradeSettings = playerService.UpgradeSettings;
        _upgradeService = playerService.UpgradeService;

        _observable = observable;
        _observable.AddObserver(this);
    }

    private void Start()
    {
        ButtonCloseFishHelper.onClick.AddListener(() => ScaleElement(false));
        
        ButtonCloseFishHelper.transform.localScale = Vector3.zero;

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

        LayoutRebuilder.ForceRebuildLayoutImmediate(gridHolder);
        Canvas.ForceUpdateCanvases();
    }
    

    public void PerformNotify(ResourceItem data)
    {
        
    }

    public override void Hide()
    {
        if(IsBlocked) return;
        
        base.Hide();    
        gameObject.SetActive(false);
    }

    public override void Show()
    {
        if(IsBlocked) return;
        
        base.Show();
        gameObject.SetActive(true);
        Canvas.ForceUpdateCanvases();
    }

    private void ScaleElement(bool isHide)
    {
        if (isHide)
        {
            _sequenceElements.Kill();
            
            _sequenceElements = DOTween.Sequence()
                .Append(_elements
                    .DOScale(0f, _durationScaleWindow)
                    .From(1f))
                .Join(_background
                    .DOFade(0.3f, _durationScaleWindow)
                    .From(0.7f))
                .Append(ButtonCloseFishHelper.transform
                    .DOScale(1f, _durationScaleWindow)
                    .From(0f));
        }
        else
        {
            _sequenceElements.Kill();
            
            _sequenceElements = DOTween.Sequence()
                .Append(ButtonCloseFishHelper.transform
                    .DOScale(0f, _durationScaleWindow)
                    .From(1f))
                .Append(_elements
                    .DOScale(1f, _durationScaleWindow)
                    .From(0f))
                .Join(_background
                    .DOFade(0.7f, _durationScaleWindow));
        }
    }

    private void OnDestroy()
    {
        ButtonCloseFishHelper.onClick.RemoveAllListeners();
        
        _elements.DOKill();
        _observable.RemoveObserver(this);
    }

    public bool IsBlocked { get; set; }
}
