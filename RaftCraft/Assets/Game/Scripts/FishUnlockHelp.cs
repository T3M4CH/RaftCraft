using DG.Tweening;
using Game.Prefabs.NPC.Vendors;
using Game.Scripts.Player.HeroPumping.Enums;
using Game.Scripts.Player.HeroPumping.Interfaces;
using Game.Scripts.Player.Spawners;
using Game.Scripts.UI.WindowManager;
using Game.Scripts.UI.WindowVendors;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

public class FishUnlockHelp : MonoBehaviour
{
    [SerializeField] private Camera _overlayCamera;
    [SerializeField] private MonoCaughtFish _caughtFish;
    [SerializeField] private Transform _lockParent;
    [SerializeField] private Transform _lockPart;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private TMP_Text _levelTMP;

    private AqualangUpgradeElement _aqualangUpgradeElement;
    private IPlayerUpgradeSettings _upgradeSettings;
    private Sequence _sequence;
    private MonoCaughtFish _currentFish;

    [Inject]
    private void Init(IPlayerService playerService, WindowManager windowManager)
    {
        _upgradeSettings = playerService.UpgradeSettings;
        _upgradeSettings.OnUpgrade += ValidatePlayerUpgrade;
        _aqualangUpgradeElement = windowManager.GetWindow<SelectorWindow>().GetComponentInChildren<AqualangUpgradeElement>();
    }

    private void Start()
    {
        _lockParent.localScale = Vector3.zero;
        SetActive(false);
    }

    private void SetActive(bool value)
    {
        _aqualangUpgradeElement.SetActiveRenderTexture(value);
        _overlayCamera.gameObject.SetActive(value);
    }

    private void ValidatePlayerUpgrade(EPlayerUpgradeType upgradeType)
    {
        if (upgradeType != EPlayerUpgradeType.FishLevel) return;

        ShowUnlockFish();
    }

    private void ShowUnlockFish()
    {
        var levelFish = _upgradeSettings.GetLevel(EPlayerUpgradeType.FishLevel);

        if (levelFish > 10) return;

        Reset();
        _sequence.Kill();

        _currentFish = Instantiate(_caughtFish, transform.position, Quaternion.identity);
        _currentFish.SetLevel(levelFish - 1);
        _currentFish.IsStatic = true;
        _levelTMP.SetText($"LVL{levelFish} \n <size=1.5><color=#FFC635>UNLOCKED!</color></size> ");

        _sequence = DOTween.Sequence()
            .AppendCallback(() => SetActive(true))
            .Append(_currentFish.transform
                .DORotate(new Vector3(0f, 630f, 0f), 1.2f, RotateMode.LocalAxisAdd)
                .SetEase(Ease.InOutBack))
            .Join(_currentFish.transform
                .DOScale(Vector3.one, 1f)
                .From(0f)
                .SetEase(Ease.OutBack))
            .Join(_levelTMP
                .DOFade(1f, 1f)
                .From(0f))
            .Append(_lockParent
                .DOScale(new Vector3(0.07f, 0.07f, 0.07f), 0.5f)
                .From(0f)
                .SetEase(Ease.OutBack))
            .Append(_lockPart
                .DOLocalRotate(new Vector3(-180f, 57f, 0f), 0.6f)
                .From(new Vector3(-180f, 0f, 0f))
                .SetEase(Ease.OutBounce))
            .AppendInterval(0.2f)
            .Append(_lockParent
                .DOScale(0f, 0.5f))
            .AppendCallback(_particleSystem.Play)
            .AppendCallback(HideFish);
    }

    private void Reset()
    {
        if (_currentFish)
        {
            Destroy(_currentFish.gameObject);
        }
    }

    private void HideFish()
    {
        _sequence.Kill();

        _sequence = DOTween.Sequence()
            .Append(_currentFish.transform
                .DOScale(Vector3.zero, 0.5f)
                .From(1f)
                .SetEase(Ease.InBack))
            .Join(_levelTMP
                .DOFade(0f, 0.5f))
            .AppendInterval(0.1f)
            .AppendCallback(() =>
            {
                Destroy(_currentFish.gameObject);
                SetActive(false);
            });
    }

    private void OnDestroy()
    {
        _upgradeSettings.OnUpgrade -= ValidatePlayerUpgrade;
        _sequence.Kill();
    }
}