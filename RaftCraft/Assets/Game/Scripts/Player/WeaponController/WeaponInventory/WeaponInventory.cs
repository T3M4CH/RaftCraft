using System.Collections.Generic;
using Game.Scripts.Core.Interface;
using Game.Scripts.Saves;
using UnityEngine;

namespace Game.Scripts.Player.WeaponController.WeaponInventory
{
    public class WeaponInventory : IInventoryWeapon, ISelectorWeapon, IGameObservable<List<WeaponItem>>, IGameObservable<WeaponItem>
    {
        private readonly GameSave _gameSave;

        private List<WeaponItem> _weapons = new List<WeaponItem>();
        
        
        private readonly List<IGameObserver<List<WeaponItem>>> _observers;
        private readonly List<IGameObserver<WeaponItem>> _observersSelectWeapon;
        
        public WeaponInventory(GameSave gameSave)
        {
            _observers = new List<IGameObserver<List<WeaponItem>>>();
            _observersSelectWeapon = new List<IGameObserver<WeaponItem>>();
            _gameSave = gameSave;
            Load();
        }
        
        private void Load()
        {
            _weapons = _gameSave.GetData(SaveConstants.WeaponInventory, new List<WeaponItem>()
            {
                new WeaponItem()
                {
                    id = WeaponId.Pistol,
                    selected = true
                }
            });
        }

        private void Save()
        {
            _gameSave.SetData(SaveConstants.WeaponInventory, _weapons);
        }

        public void AddWeapon(WeaponId id)
        {
            if(id == WeaponId.Minigun) return;
            
            if (HaveContains(id))
            {
                return;
            }

            var last = GetCurrentSelected();
            if (last != null)
            {
                last.selected = false;
            }
            
            _weapons.Add(new WeaponItem()
            {
                id = id,
                selected = true
            });
            
            Save();
            Notify(_weapons);
            Notify(GetCurrentSelected());
        }

        public WeaponItem GetCurrentSelected()
        {
            foreach (var weapon in _weapons)
            {
                if (weapon.selected)
                {
                    return weapon;
                }
            }

            return null;
        }

        private bool HaveContains(WeaponId id)
        {
            foreach (var weapon in _weapons)
            {
                if (weapon.id == id)
                {
                    return true;
                }
            }

            return false;
        }

        private WeaponItem GetItem(WeaponId id)
        {
            foreach (var weapon in _weapons)
            {
                if (weapon.id == id)
                {
                    return weapon;
                }
            }

            return null;
        }

        public void SelectWeapon(WeaponId id)
        {
            var weapon = GetItem(id);
            if (weapon == null)
            {
                return;
            }

            var last = GetCurrentSelected();
            if (last != null)
            {
                last.selected = false;
            }
            
            weapon.selected = true;
            Notify(_weapons);
            Notify(weapon);
            Save();
        }

        public void AddObserver(IGameObserver<List<WeaponItem>> gameObserver)
        {
            _observers.Add(gameObserver);
            gameObserver.PerformNotify(_weapons);
        }

        public void RemoveObserver(IGameObserver<List<WeaponItem>> gameObserver)
        {
            _observers.Remove(gameObserver);
        }

        public void Notify(List<WeaponItem> data)
        {
            foreach (var observer in _observers)
            {
                if (observer != null)
                {
                    observer.PerformNotify(data);
                }
            }
        }

        public void AddObserver(IGameObserver<WeaponItem> gameObserver)
        {
            _observersSelectWeapon.Add(gameObserver);
            var last = GetCurrentSelected();
            if (last != null)
            {
                gameObserver.PerformNotify(last);
            }
        }

        public void RemoveObserver(IGameObserver<WeaponItem> gameObserver)
        {
            _observersSelectWeapon.Remove(gameObserver);
        }

        public void Notify(WeaponItem data)
        {
            foreach (var observer in _observersSelectWeapon)
            {
                if (observer != null)
                {
                    observer.PerformNotify(data);
                }
            }
        }
    }
}
