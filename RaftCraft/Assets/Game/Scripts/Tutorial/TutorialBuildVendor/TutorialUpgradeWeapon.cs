using System;
using System.Linq;
using DG.Tweening;
using Game.Prefabs.NPC.Vendors;
using Game.Scripts.Core.Interface;
using Game.Scripts.Player.WeaponController;
using Game.Scripts.Player.WeaponController.WeaponsData;
using Game.Scripts.Saves;
using Game.Scripts.ShopVendor;
using Game.Scripts.Tutorial.TutorialBuildVendor;
using Game.Scripts.UI.WindowManager;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

public class TutorialUpgradeWeapon : TutorialBuildVendor, IGameObserver<WeaponUpgrade>
{
    [SerializeField, FoldoutGroup("Settings")] private WeaponsDataSettings _settings;
    [SerializeField, FoldoutGroup("UI")] private RectTransform _cursor;

    [SerializeField, FoldoutGroup("Feedback")] private float _duration;
    [SerializeField, FoldoutGroup("Feedback")] private Vector3 _sizeScale;
    [SerializeField, FoldoutGroup("Feedback")] private Ease _ease;
    
    private IGameObservable<WeaponUpgrade> _observable;
    private ShopVendorController _shop;
    private PlayerUpgradesVendor _targetVendor;
    private WindowManager _windowManager;
    
    [Inject]
    private void Construct(IGameObservable<WeaponUpgrade> observable, WindowManager windowManager)
    {
        _observable = observable;
        _windowManager = windowManager;
    }

    private void Start()
    {
        _targetVendor = FindObjectsOfType<PlayerUpgradesVendor>().Where(tile => tile.name == "WeaponShop").OrderBy(x => x.gameObject.activeSelf).First();
        _shop = FindObjectsOfType<ShopVendorController>().Where(tile => tile.name == "ShopControllerWeapon").OrderBy(x => x.gameObject.activeSelf).First();
    }

    public override int Cost()
    {
        return _settings.Cost(1);
    }

    public override void StartTutorial()
    {
        _windowManager.SetNameTutorial("Upgrade weapon");
        _observable.AddObserver(this);
        _targetVendor.OnEnterInteraction += TargetVendorOnOnEnterInteraction;
        _targetVendor.OnExitInteraction += TargetVendorOnOnExitInteraction;
    }

    private void TargetVendorOnOnExitInteraction()
    {
        Debug.Log($"Exit Player");
    }

    private void TargetVendorOnOnEnterInteraction()
    {
        if (_cursor == null)
        {
            return;
        }
        
        _cursor.gameObject.SetActive(true);
        _cursor.DOPunchScale(_sizeScale, _duration, 1).SetEase(_ease).SetLoops(-1);
    }

    public override void Complete(GameSave gameSave)
    {
        base.Complete(gameSave);
        _windowManager.SetNameTutorial("");
        if (_cursor != null)
        {
            _cursor.gameObject.SetActive(false);
        }
        _targetVendor.OnEnterInteraction -= TargetVendorOnOnEnterInteraction;
        _targetVendor.OnExitInteraction -= TargetVendorOnOnExitInteraction;
    }

    public override Transform Target()
    {
        return _shop.transform;
    }

    public void PerformNotify(WeaponUpgrade data)
    {
        if (data.Id != _settings.CurrentId)
        {
            return;
        }

        if (data.Level >= 1)
        {
            Debug.Log("Complete Upgrade");
            OnComplete?.Invoke(this);
        }
    }

    private void OnDestroy()
    {
        _targetVendor.OnEnterInteraction -= TargetVendorOnOnEnterInteraction;
        _targetVendor.OnExitInteraction -= TargetVendorOnOnExitInteraction;
        _observable.RemoveObserver(this);
    }
}
