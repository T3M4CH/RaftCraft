using System;
using System.Collections.Generic;
using Game.Scripts.Core.Interface;
using Game.Scripts.Days;
using Game.Scripts.Player.WeaponController.WeaponInventory;
using Game.Scripts.ResourceController.Interfaces;
using Game.Scripts.Saves;
using GTap.Analytics;
using UnityEngine;

namespace Game.Scripts.Player.WeaponController.WeaponShop
{
    public class WeaponShopController : IShopView, IDisposable, IWeaponShop
    {
        public event Action<WeaponPrice> OnBuyWeapon; 

        private int _levelUnLock;

        private int LevelUnLock
        {
            get => _levelUnLock;
            set => _levelUnLock = value;
        }

        private List<IGameObserver<WeaponPrice>> _observers;
        
        private readonly WeaponShopData _data;
        private readonly IDayService _dayService;
        private readonly GameSave _gameSave;
        private readonly IResourceService _resourceService;
        private readonly IInventoryWeapon _inventoryWeapon;
        
        public WeaponShopController(WeaponShopData data, IDayService dayService, GameSave gameSave, IResourceService resourceService, IInventoryWeapon inventoryWeapon)
        {
            _inventoryWeapon = inventoryWeapon;
            _observers = new List<IGameObserver<WeaponPrice>>();
            _data = data;
            _dayService = dayService;
            _gameSave = gameSave;
            _resourceService = resourceService;
            LoadData();
        }
        
        public void AddObserver(IGameObserver<WeaponPrice> gameObserver)
        {
            _observers.Add(gameObserver);
            gameObserver.PerformNotify(TryGetCurrentWeapon(out var weapon) ? weapon : null);
        }

        public void RemoveObserver(IGameObserver<WeaponPrice> gameObserver)
        {
            _observers.Remove(gameObserver);
        }

        public void Notify(WeaponPrice data)
        {
            foreach (var observer in _observers)
            {
                if (observer == null)
                {
                    continue;
                }
                
                observer.PerformNotify(data);
            }
        }

        private void LoadData()
        {
            if (_gameSave == null)
            {
                return;
            }
            LevelUnLock = _gameSave.GetData(SaveConstants.WeaponShop, 0);
            _dayService.OnDayStart += DayServiceOnOnDayStart;
        }

        private void DayServiceOnOnDayStart(int obj)
        {
            if (TryGetCurrentWeapon(out var weapon))
            {
                Notify(weapon);
            }
        }

        private void SaveData()
        {
            if (_gameSave == null)
            {
                return;
            }
            _gameSave.SetData(SaveConstants.WeaponShop, LevelUnLock);
        }

        public void Dispose()
        {
            _dayService.OnDayStart -= DayServiceOnOnDayStart;
            SaveData();
        }

        public bool TryGetCurrentWeapon(out WeaponPrice result)
        {
            if (_data == null)
            {
                result = null;
                return false;
            }
            
            if (_data.TryGetTrice(LevelUnLock, out var price))
            {
                result = price;
                return true;
            }

            result = null;
            return false;   
        }

        public bool HaveUnLockCurrent()
        {
            if (_data == null || _dayService == null)
            {
                return false;
            }

            if (!TryGetCurrentWeapon(out var price)) return false;
            return price.DayUnLock <= _dayService.CurrentDay;
        }

        public bool HaveBuyCurrent()
        {
            if (_resourceService == null)
            {
                return false;
            }
            return TryGetCurrentWeapon(out var weapon) && _resourceService.HaveCount(weapon.ItemType, weapon.Cost) && HaveUnLockCurrent();
        }

        public bool TryBuyWeapon()
        {
            if (HaveBuyCurrent() == false)
            {
                return false;
            }
            if (!TryGetCurrentWeapon(out var weapon)) return false;
            if (!_resourceService.HaveCount(weapon.ItemType, weapon.Cost)) return false;
            _resourceService.TryRemove(weapon.ItemType, weapon.Cost);
            _inventoryWeapon.AddWeapon(weapon.Id);
            LevelUnLock++;
            GtapAnalytics.BuyWeapon(weapon.Id.ToString());
            OnBuyWeapon?.Invoke(weapon);
            Notify(TryGetCurrentWeapon(out var next) ? next : null);
            SaveData();
            return true;
        }
    }
}
