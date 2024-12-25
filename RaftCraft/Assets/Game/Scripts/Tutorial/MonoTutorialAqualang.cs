using Game.Scripts.UI.WindowManager;
using Game.Scripts.Raft.BuildSystem;
using Game.Scripts.Player.Spawners;
using Game.Scripts.Raft.Interface;
using Game.Scripts.GameIndicator;
using Cysharp.Threading.Tasks;
using Game.Scripts.Saves;
using UnityEngine;
using System;
using DG.Tweening;
using Game.Prefabs.NPC.Vendors;
using Game.Scripts.CollectingResources;
using Game.Scripts.Player.HeroPumping.Enums;
using Game.Scripts.Player.HeroPumping.Interfaces;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ResourceController.Interfaces;

public class MonoTutorialAqualang : MonoTutorialBase
{
    [SerializeField] private MonoArrowCanvas _arrowCanvas;
    [SerializeField] private MonoArrowCanvas _upgradeArrow;
    [SerializeField] private RectTransform _cursor;

    private bool _isEndBuild;
    private GameSave _gameSave;
    private Sequence _sequence;
    private Sprite _buildSprite;
    private TileBuild _currentTile;
    private IRaftStructures _raftStructures;
    private IControllerIndicator _controllerIndicator;
    private WindowScubaUpgrade _scubaWindow;
    private ScubaUpgradeVendor _scubaUpgradeVendor;
    private IResourceService _resourceService;
    private IPlayerUpgradeSettings _upgradeSettings;
    private GameObject _scubaVendor;

    private const string RaftStructures = "_raftStructures";
    private const string ControllerIndicator = "_controllerIndicator";
    private const string ResourceService = "_resourceService";

    public override void Initialize(GameSave gameSave, IPlayerService playerSpawner, WindowManager windowManager, object extraParams)
    {
        _gameSave = gameSave;
        if (_gameSave.HaveKey(SaveConstants.TutorialSix))
        {
            ShowBattleButton(true);
            Destroy(gameObject);
            return;
        }

        ShowBattleButton(false);

        try
        {
            var type = extraParams.GetType();
            _raftStructures = (IRaftStructures)type.GetProperty(RaftStructures)?.GetValue(extraParams);
            _controllerIndicator = (IControllerIndicator)type.GetProperty(ControllerIndicator)?.GetValue(extraParams);
            _resourceService = (IResourceService)type.GetProperty(ResourceService)?.GetValue(extraParams);

            if (_raftStructures == null || _controllerIndicator == null || _resourceService == null)
            {
                throw new Exception("Fail cast");
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        var addValue = playerSpawner.UpgradeService.GetPrice(EPlayerUpgradeType.Oxygen) - _resourceService.GetValue(EResourceType.CoinBlue);
        _resourceService.Add(EResourceType.CoinBlue, addValue);
        _upgradeSettings = playerSpawner.UpgradeSettings;
        _upgradeSettings.OnUpgrade += ValidateUpgrade;
        _scubaWindow = windowManager.GetWindow<WindowScubaUpgrade>();
        _scubaWindow.OnUpgrade += Complete;
        var iconsConfig = Resources.Load<IconConfig>("IconConfig");
        _buildSprite = iconsConfig.HammerIcon;
    }

    private void ResetStep()
    {
        _currentTile.OnBuyTile -= PerformBuildTile;

        _controllerIndicator.RemoveTarget(_currentTile.UITransform);
        _arrowCanvas.gameObject.SetActive(false);

        if (_isEndBuild)
        {
            ShowScubaVendor();
        }
    }

    private async void PerformBuildTile()
    {
        if (_currentTile)
        {
            ResetStep();
        }

        if (_isEndBuild) return;

        await UniTask.WaitUntil(GetTile, PlayerLoopTiming.FixedUpdate, this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();

        SetStep();
    }

    private bool GetTile()
    {
        try
        {
            _currentTile = _raftStructures.GetActiveTile(out var row, out var column);

            if (row == 2)
            {
                if (column == 1)
                {
                    _currentTile.OnBuyTile += PerformLastBuild;
                }
                else
                {
                    ShowScubaVendor();
                    _currentTile = null;
                }
            }

            return true;
        }
        catch (Exception _)
        {
            return false;
        }
    }

    private void SetStep()
    {
        if (_currentTile == null || _isEndBuild) return;

        var target = _currentTile.UITransform;
        _controllerIndicator.AddTarget(target, new Vector3(800f, 400f, 0f), 15, _buildSprite, new Color(0.07f, 0.07f, 0.06f), multiplierOffset: 2f);
        _arrowCanvas.transform.position = target.position;
        _arrowCanvas.gameObject.SetActive(true);

        _currentTile.OnBuyTile += PerformBuildTile;
    }

    private void PerformLastBuild()
    {
        _isEndBuild = true;

        _currentTile.OnBuyTile -= PerformLastBuild;
        _currentTile.OnBuyTile -= PerformBuildTile;

        ShowScubaVendor();
    }

    private void ShowScubaVendor()
    {
        _scubaUpgradeVendor = FindObjectOfType<ScubaUpgradeVendor>();
        _scubaVendor = _scubaUpgradeVendor.transform.Find("VendorParent").gameObject;
        _upgradeArrow.gameObject.SetActive(true);
        var position = _scubaUpgradeVendor.transform.position;
        position.y += 4.4f;
        position.x += 1f;
        _upgradeArrow.transform.position = position;

        if (!_scubaVendor.activeInHierarchy)
        {
            _upgradeArrow.ArrowText.text = "Buy";
        }
        else
        {
            _upgradeArrow.ArrowText.text = "Upgrade";
        }

        //TODO: вернуться
        _scubaUpgradeVendor.OnEnterInteraction += HideArrow;
        _scubaUpgradeVendor.OnExitInteraction += ShowArrow;
        //_scubaUpgradeVendor.OnWindowOpen += PerformOpenWindow;
    }

    private void ShowArrow() => ValidateUpgradeArrow(true);
    private void HideArrow() => ValidateUpgradeArrow(false);

    private void ValidateUpgradeArrow(bool value)
    {
        if (!_scubaVendor.activeInHierarchy)
        {
            _upgradeArrow.ArrowText.text = "Buy";
        }
        else
        {
            _upgradeArrow.ArrowText.text = "Upgrade";
        }

        if (_upgradeArrow)
        {
            _upgradeArrow.gameObject.SetActive(value);
        }
    }

    private void ValidateUpgrade(EPlayerUpgradeType upgradeType)
    {
        if (upgradeType == EPlayerUpgradeType.Oxygen)
        {
            Complete();
        }
    }

    private void PerformOpenWindow()
    {
        //_scubaUpgradeVendor.OnWindowOpen -= PerformOpenWindow;

        var addValue = 30 - _resourceService.GetValue(EResourceType.CoinGold);
        _resourceService.Add(EResourceType.CoinGold, addValue);

        _scubaWindow.IsBlocked = true;

        _cursor.gameObject.SetActive(true);

        _sequence.Kill();
        _sequence = DOTween.Sequence();
        _sequence.Append(_cursor.DOScale(0.95f, 1).SetLoops(int.MaxValue, LoopType.Yoyo));
    }

    protected override void Complete()
    {
        base.Complete();

        _sequence?.Kill();
        _scubaWindow.IsBlocked = false;
        _scubaWindow.OnUpgrade -= Complete;
        _gameSave.SetData(SaveConstants.TutorialSix, true);
        Destroy(gameObject);
    }

    private void Start()
    {
        PerformBuildTile();
    }

    private void OnDestroy()
    {
        _sequence.Kill();

        if (_upgradeSettings != null)
        {
            _upgradeSettings.OnUpgrade -= ValidateUpgrade;
        }

        if (_currentTile)
        {
            _currentTile.OnBuyTile -= PerformLastBuild;
            _currentTile.OnBuyTile -= PerformBuildTile;
        }

        if (_scubaUpgradeVendor)
        {
            _scubaUpgradeVendor.OnEnterInteraction -= HideArrow;
            _scubaUpgradeVendor.OnExitInteraction -= ShowArrow;
        }

        if (_scubaWindow)
        {
            _scubaWindow.OnUpgrade -= Complete;
        }
    }
}