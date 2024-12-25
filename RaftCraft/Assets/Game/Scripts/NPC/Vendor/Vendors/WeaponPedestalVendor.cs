using System;
using Cysharp.Threading.Tasks;
using Game.Prefabs.NPC.Vendors;
using Game.Scripts.Core.Interface;
using Game.Scripts.Days;
using Game.Scripts.Player.WeaponController;
using Game.Scripts.Player.WeaponController.WeaponShop;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

public class WeaponPedestalVendor : PlayerUpgradesVendor, IGameObserver<WeaponPrice>
{
    [System.Serializable]
    public class WeaponView
    {
        public WeaponId id;
        public GameObject link;
    }

    [SerializeField] private LayerMask layerMask;
    [SerializeField] private TMP_Text _unlockText;
    [SerializeField] private WeaponView[] _guns;
    [SerializeField] private FrightenedObject frightenedObject;
    [SerializeField] private Transform rackPositionOne;
    [SerializeField] private Transform rackPositionTwo;

    private IShopView _shopView;
    private IDayService _dayService;
    private IGameObservable<WeaponPrice> _gameObservable;

    [Inject]
    private void Construct(IGameObservable<WeaponPrice> observable, IShopView shopView, IDayService dayService)
    {
        _shopView = shopView;
        _dayService = dayService;
        _dayService.OnDayStart += ValidateDay;
        _gameObservable = observable;
        _gameObservable.AddObserver(this);
    }

    private async void ValidateDay(int dayId)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        var condition = dayId >= 5;
        frightenedObject.IsBlock = !condition;
        var ray = new Ray(transform.position + Vector3.up, -transform.up);

        if (condition && Physics.Raycast(ray, 3, layerMask))
        {
            transform.localScale = Vector3.one;
        }
        else
        {
            transform.localScale = Vector3.zero;
        }

        gameObject.SetActive(condition);
    }

    public void PerformNotify(WeaponPrice data)
    {
        if (data == null)
        {
            _unlockText.enabled = false;
            foreach (var gun in _guns)
            {
                gun.link.SetActive(false);
            }

            Debug.LogWarning("WeaponPedestalDestroy");
            Destroy(frightenedObject);
            gameObject.SetActive(false);

            return;
        }

        transform.position = data.Id == WeaponId.Pph ? rackPositionOne.position : rackPositionTwo.position;
        
        ValidateDay(_dayService.CurrentDay);
        
        _unlockText.text = _shopView.HaveUnLockCurrent() ? "UnLocked!" : $"Unlock on Day {data.DayUnLock}";

        foreach (var gun in _guns)
        {
            gun.link.SetActive(gun.id == data.Id);
        }
    }

    private void OnDestroy()
    {
        _dayService.OnDayStart -= ValidateDay;
    }
}