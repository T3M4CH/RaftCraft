using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Scripts.Core.Interface;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Player.WeaponController.WeaponInventory.UI
{
    public class WeaponInventoryView : MonoBehaviour, IGameObserver<List<WeaponItem>>
    {
        [SerializeField, FoldoutGroup("UI")] private RectTransform _rectTransform;
        
        [SerializeField, FoldoutGroup("Settings")]
        private RectTransform _contentItems;

        [SerializeField, FoldoutGroup("Settings")]
        private WeaponInventoryCell _prefabCell;

        private IGameObservable<List<WeaponItem>> _observable;
        private ISelectorWeapon _selector;
        private Dictionary<WeaponId, WeaponInventoryCell> _poolCell = new Dictionary<WeaponId, WeaponInventoryCell>();

        [Inject]
        private void Construct(IGameObservable<List<WeaponItem>> observable, ISelectorWeapon selectorWeapon)
        {
            _observable = observable;
            _selector = selectorWeapon;
            _observable.AddObserver(this);
        }
        
        public void SetActiveWindow(bool value)
        {
            _rectTransform.DOKill();

            _rectTransform.DOScale(value ? Vector3.one : Vector3.zero, 0.2f);
        
        }

        public void PerformNotify(List<WeaponItem> data)
        {
            foreach (var weapon in data)
            {
                if (_poolCell.ContainsKey(weapon.id) == false)
                {
                    _poolCell.Add(weapon.id, CreateCell(weapon.id));
                }

                if (weapon.selected)
                {
                    _poolCell[weapon.id].Select();
                }
                else
                {
                    _poolCell[weapon.id].UnSelect();
                }
            }
            gameObject.SetActive(_poolCell.Count > 1);
        }

        private WeaponInventoryCell CreateCell(WeaponId id)
        {
            var cell = Instantiate(_prefabCell, _contentItems);
            cell.OnClick += CellOnOnClick;
            cell.Init(id);
            return cell;
        }

        private void CellOnOnClick(WeaponId obj)
        {
            if (_selector == null)
            {
                return;
            }
            
            _selector.SelectWeapon(obj);
        }

        private void OnDestroy()
        {

            foreach (var cell in _poolCell)
            {
                cell.Value.OnClick -= CellOnOnClick;
            }
            if (_observable != null)
            {
                _observable.RemoveObserver(this);
            }
        }
    }
}