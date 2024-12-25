using System;
using DG.Tweening;
using Game.Scripts.Core.Interface;
using Game.Scripts.ResourceController.Enums;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.ResourceController.UI
{
    public class ResourcesCell : MonoBehaviour
    {
        public enum CellState
        {
            None,
            Open,
            Hide
        }
        [SerializeField, FoldoutGroup("UI")] private RectTransform _rect;
        [SerializeField, FoldoutGroup("UI")] private CanvasGroup _group;
        
        [field:SerializeField, FoldoutGroup("UI")] public Image Icon { get; private set; }
        [SerializeField, FoldoutGroup("UI")] private TextMeshProUGUI _textCount;
        [SerializeField, FoldoutGroup("Settings")] private IconConfig _config;
        [SerializeField, FoldoutGroup("Feedback")] private Vector3 _punchScale;
        [SerializeField, FoldoutGroup("Feedback")] private float _duration;

        [SerializeField, FoldoutGroup("Feedback Show")] private float _durationShow;
        [SerializeField, FoldoutGroup("Feedback Show")] private Ease _easeShow;
        
        private EResourceType _resourceType;
        [SerializeField, ReadOnly]
        private Vector2 _sizeDelta;

        private CellState _state;

        private CellState State
        {
            get => _state;
            set
            {
                if (_state == value)
                {
                    return;
                }
                _state = value;
                switch (_state)
                {
                    case CellState.None:
                        break;
                    case CellState.Open:
                        Show();
                        break;
                    case CellState.Hide:
                        Hide();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }


        private void OnValidate()
        {
            _sizeDelta = _rect.sizeDelta;
        }

        public void InitIcon(EResourceType type)
        {
            _resourceType = type;
            Icon.sprite = _config.GetIconItem(type);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rect);
            Canvas.ForceUpdateCanvases();
        }

        [Button]
        public void Show()
        {
            gameObject.SetActive(true);
            _rect.DOKill();
            _group.DOKill();
            _group.alpha = 0;
            _group.DOFade(1f, _durationShow).SetEase(_easeShow);
            _rect.sizeDelta = new Vector2(0f, _sizeDelta.y);
            _rect.DOSizeDelta(_sizeDelta, _durationShow).SetEase(_easeShow);
        }

        [Button]
        public void Hide(bool instantly = false)
        {
            if (instantly)
            {
                gameObject.SetActive(false);
                return;
            }
            
            _rect.DOKill();
            _group.DOKill();
            _group.DOFade(0f, _durationShow).SetEase(_easeShow);
            _rect.DOSizeDelta(new Vector2(0f, _sizeDelta.y), _durationShow).SetEase(_easeShow).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
        
        public void Notify(ResourceItem data)
        {
            if (data.Type == EResourceType.CoinGold)
            {
                transform.SetSiblingIndex(0);
            }
            _textCount.text = string.Empty;
            _textCount.rectTransform.DOKill();
            _textCount.rectTransform.localScale = Vector3.one;
            _textCount.rectTransform.DOPunchScale(_punchScale, _duration, 1);
            if (data.TempCount != 0)
            {
                _textCount.text += $"<color=#87F115>{data.TempCount}+</color>";
            }
            _textCount.text += $"{data.Count}";

            if (IsUsualFish(data))
            {
                Hide(true);
                return;
            }
            
            if (data.Count == 0 && data.TempCount == 0)
            {
                State = CellState.Hide;
            }
            else
            {
                State = CellState.Open;
            }
        }

        private bool IsUsualFish(ResourceItem data)
        {
            var value = (int)data.Type;
            
            return value is >= 2 and <= 12;
        }
    }
}
