using Game.Scripts.Core.Interface;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.Scripts.Player.WeaponController.WeaponShop.ViewUI
{
    public class CellWeaponBuy : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("UI")] private Image _imageButton;
        [SerializeField, FoldoutGroup("UI")] private Image _iconWeapon;
        [SerializeField, FoldoutGroup("UI")] private TextMeshProUGUI _textCost;
        [SerializeField, FoldoutGroup("UI")] private Image _iconItem;
        [field: SerializeField, FoldoutGroup("UI")] public Button ButtonBuy { get; private set; }

        [SerializeField, FoldoutGroup("Settings")]
        private RectTransform _rectCost;

        [SerializeField, FoldoutGroup("Settings")]
        private IconConfig _iconConfig;

        [SerializeField, FoldoutGroup("Settings")]
        private Sprite _spriteButtonUnLock;

        [SerializeField, FoldoutGroup("Settings")]
        private Sprite _spriteButtonLock;

        private WeaponPrice _currentPrice;

        public WeaponId CurrentId => _currentPrice.Id;

        public void Init(WeaponPrice data)
        {
            if (data == null)
            {
                return;
            }

            _currentPrice = data;
            _iconWeapon.sprite = _iconConfig.GetIconWeapon(data.Id);
            if (data.Cost > 0)
            {
                _iconItem.sprite = _iconConfig.GetIconItem(data.ItemType);
                _textCost.text = data.Cost.ToString();
            }
            else
            {
                _iconItem.gameObject.SetActive(false);
                _textCost.text = "FREE";
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectCost);
            Canvas.ForceUpdateCanvases();
        }

        public void UpdateStateLock(bool lockState)
        {
            //ButtonBuy.interactable = lockState;
            _imageButton.sprite = lockState ? _spriteButtonUnLock : _spriteButtonLock;
        }
    }
}