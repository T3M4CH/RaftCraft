using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Player.WeaponController.WeaponInventory.UI
{
    public class WeaponInventoryCell : MonoBehaviour
    {
        public event Action<WeaponId> OnClick; 
        
        [SerializeField, FoldoutGroup("UI")] private Image _imageButton;
        [SerializeField, FoldoutGroup("UI")] private Image _imageIconWeapon;
        [SerializeField, FoldoutGroup("UI")] private RectTransform _content;
        
        [SerializeField, FoldoutGroup("Settings")]
        private Sprite _spriteSelect;

        [SerializeField, FoldoutGroup("Settings")]
        private Sprite _spriteUnSelect;

        [SerializeField, FoldoutGroup("Feedback")]
        private float _durationSelect;

        [SerializeField, FoldoutGroup("Feedback")]
        private Ease _easeSelect;

        [SerializeField, FoldoutGroup("Feedback")]
        private Vector3 _positionSelect;

        [SerializeField, FoldoutGroup("Components")]
        private IconConfig _config;

        private WeaponId _currentId;
        
        public void Init(WeaponId id)
        {
            _currentId = id;
            _imageIconWeapon.sprite = _config.GetIconWeapon(id);
        }
        
        [Button]
        public void Select()
        {
            _imageButton.sprite = _spriteSelect;
            _content.DOKill();
            _content.DOAnchorPos(_positionSelect, _durationSelect).SetEase(_easeSelect);
        }

        [Button]
        public void UnSelect()
        {
            _imageButton.sprite = _spriteUnSelect;
            _content.DOKill();
            _content.DOAnchorPos(Vector2.zero, _durationSelect).SetEase(_easeSelect);
        }

        public void Click()
        {
            OnClick?.Invoke(_currentId);
        }
    }
}