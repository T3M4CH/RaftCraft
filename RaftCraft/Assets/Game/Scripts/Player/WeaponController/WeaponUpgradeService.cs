using System;
using System.Collections.Generic;
using Game.Scripts.Core.Interface;
using Game.Scripts.Player.WeaponController.Interface;
using Game.Scripts.Player.WeaponController.WeaponsData;
using Game.Scripts.Player.WeaponController.WeaponShop;
using Game.Scripts.Saves;
using GTap.Analytics;

namespace Game.Scripts.Player.WeaponController
{
    [System.Serializable]
    public class WeaponUpgrade
    {
        public WeaponId Id;
        public int Level;
        public bool UnLock;
    }

    public class WeaponUpgradeService : IGameObservable<WeaponUpgrade>, IWeaponUpgradeService, IDisposable
    {
        private readonly GameSave _gameSave;

        private readonly Dictionary<WeaponId, WeaponUpgrade> _upgrades;

        private readonly List<IGameObserver<WeaponUpgrade>> _observers;

        private List<WeaponsDataSettings> _allWeapons;
        private IShopView _shopView;

        public WeaponUpgradeService(GameSave gameSave, List<WeaponsDataSettings> allWeapons, IShopView shopView)
        {
            _observers = new List<IGameObserver<WeaponUpgrade>>();
            _upgrades = new Dictionary<WeaponId, WeaponUpgrade>();
            _gameSave = gameSave;
            _allWeapons = allWeapons;
            _shopView = shopView;
            foreach (var id in (WeaponId[])Enum.GetValues(typeof(WeaponId)))
            {
                var data = _gameSave.GetData($"Weapon_{id}", new WeaponUpgrade()
                {
                    Id = id,
                    Level = 0,
                    UnLock = id == WeaponId.Pistol
                });
                _upgrades.TryAdd(id, data);
            }

            NotifyAll();
            _shopView.OnBuyWeapon += ShopViewOnOnBuyWeapon;
        }

        private void ShopViewOnOnBuyWeapon(WeaponPrice weapon)
        {
            if (_upgrades.TryGetValue(weapon.Id, out var upgrade))
            {
                upgrade.UnLock = true;
                _gameSave.SetData($"Weapon_{upgrade.Id}", _upgrades[upgrade.Id]);
                NotifyAll();
            }
        }

        public event Action OnUpgrade;

        public void Upgrade(WeaponId weaponId)
        {
            _upgrades.TryAdd(weaponId, new WeaponUpgrade()
            {
                Id = weaponId,
                Level = 0
            });

            _upgrades[weaponId].Level++;
            _upgrades[weaponId].UnLock = true;

            GtapAnalytics.Upgrade(_upgrades[weaponId].Level, weaponId.ToString());
            _gameSave.SetData($"Weapon_{weaponId}", _upgrades[weaponId]);
            OnUpgrade?.Invoke();
            Notify(_upgrades[weaponId]);
        }

        public int CostUpgrade(WeaponId id)
        {
            foreach (var weapon in _allWeapons)
            {
                if (weapon.CurrentId == id)
                {
                    return weapon.Cost(_upgrades[id].Level);
                }
            }

            return 0;
        }

        public void AddObserver(IGameObserver<WeaponUpgrade> gameObserver)
        {
            if (_observers.Contains(gameObserver))
            {
                return;
            }

            _observers.Add(gameObserver);
            foreach (var upgrade in _upgrades)
            {
                gameObserver.PerformNotify(upgrade.Value);
            }
        }

        private void NotifyAll()
        {
            foreach (var observer in _observers)
            {
                foreach (var upgrade in _upgrades)
                {
                    if (observer == null)
                    {
                        continue;
                    }

                    observer.PerformNotify(upgrade.Value);
                }
            }
        }

        public void RemoveObserver(Core.Interface.IGameObserver<WeaponUpgrade> gameObserver)
        {
            if (_observers.Contains(gameObserver) == false)
            {
                return;
            }

            _observers.Remove(gameObserver);
        }

        public void Notify(WeaponUpgrade weaponUpgrade)
        {
            foreach (var observer in _observers)
            {
                observer.PerformNotify(weaponUpgrade);
            }
        }

        public void Dispose()
        {
            if (_shopView == null)
            {
                return;
            }

            _shopView.OnBuyWeapon -= ShopViewOnOnBuyWeapon;
        }
    }
}