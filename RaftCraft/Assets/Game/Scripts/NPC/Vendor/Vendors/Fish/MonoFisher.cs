using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Prefabs.NPC.Vendors;
using Game.Scripts.ResourceController;
using Game.Scripts.Core.Interface;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ShopController;
using Reflex.Attributes;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonoFisher : MonoBehaviour, IGameObserver<ResourceItem>
{
    [SerializeField] private float delay;
    [SerializeField] private Image fillImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text textPrice;
    [SerializeField] private GameObject indicator;
    [SerializeField] private MonoMoneyPot moneyPot;
    [SerializeField] private MonoNetController netController;
    [SerializeField] private GameObject[] fishLevels;

    private int _loops;
    private bool _isConverting;
    private float _currentTime;
    public int generalLoops;
    private float _passedTime;
    private float _generalTimer;
    private ShopData _shopData;
    private IconConfig _iconConfig;
    private MonoCaughtFish _caughtFish;
    private IGameObservable<ResourceItem> _resourceObservable;

    [Inject]
    private void Construct(IGameObservable<ResourceItem> resObservable)
    {
        _resourceObservable = resObservable;

        _shopData = Resources.Load<ShopData>("Shop/ShopFish");
        _iconConfig = Resources.Load<IconConfig>("IconConfig");
    }

    public async void PerformNotify(ResourceItem data)
    {
        UpdateData();

        if (!_isConverting)
        {
            await UniTask.Yield(PlayerLoopTiming.LastUpdate, this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();

            if (_isConverting) return;
            _caughtFish = netController.TakeFish();

            if (_caughtFish)
            {
                fishLevels.ForEach(fish => fish.SetActive(false));
                fishLevels[_caughtFish.Level - 1].SetActive(true);
                _isConverting = true;
                indicator.SetActive(true);
                //var parse = Enum.Parse<EResourceType>($"FishLvl{_caughtFish.Level}");
                iconImage.sprite = _iconConfig.GetIconItem(EResourceType.UniversalFish);
                _loops = _shopData.Price[_caughtFish.Level - 1].CountOutput;
            }
            else
            {
                _passedTime = 0;
                _generalTimer = 0;
                indicator.SetActive(false);
            }
        }
    }

    private void PerformConvert()
    {
        _currentTime = 0;

        _isConverting = false;

        if (_caughtFish == null) return;

        PerformNotify(null);
    }

    private void AddMoney()
    {
        _currentTime = 0;

        moneyPot.AddMoney(1);
    }

    private void UpdateData()
    {
        var price = netController.Fishes.Sum(fish => _shopData.Price[fish.Level - 1].CountOutput) + _loops;
        textPrice.text = price.ToString();
    }

    public void AddTime(int level)
    {
        _generalTimer += _shopData.Price[level].CountOutput * delay;
    }

    private void CalculateTime()
    {
        var price = netController.Fishes.Sum(fish => _shopData.Price[fish.Level - 1].CountOutput) + _loops;
        _generalTimer = price * delay;
    }

    private void Update()
    {
        if (_isConverting)
        {
            _currentTime += Time.deltaTime;
            _passedTime += Time.deltaTime;

            fillImage.fillAmount = _passedTime / _generalTimer;

            if (_currentTime > delay)
            {
                _loops -= 1;

                AddMoney();
                UpdateData();

                if (_loops <= 0)
                {
                    PerformConvert();
                }
            }
        }
    }

    private async void Start()
    {
        _resourceObservable.AddObserver(this);

        await UniTask.Delay(TimeSpan.FromSeconds(1)).SuppressCancellationThrow();

        CalculateTime();
        PerformNotify(null);
    }

    private void OnDestroy()
    {
        _resourceObservable.RemoveObserver(this);
    }
}