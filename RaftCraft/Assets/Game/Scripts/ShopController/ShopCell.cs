using System;
using System.Collections;
using DG.Tweening;
using Game.Scripts.Core.Interface;
using Game.Scripts.ResourceController;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.ShopController
{
    public class ShopCell : MonoBehaviour, IGameObserver<ResourceItem>
    {
        public event Action OnClickEvent;
        
        [SerializeField, FoldoutGroup("UI")] private Image _iconInput;
        [SerializeField, FoldoutGroup("UI")] private Image[] _iconOutput;
        [SerializeField, FoldoutGroup("UI")] private TextMeshProUGUI _textCountInput;
        [SerializeField, FoldoutGroup("UI")] private TextMeshProUGUI _textCountOutput;
        [SerializeField, FoldoutGroup("UI")] private TextMeshProUGUI _textCost;
        [SerializeField, FoldoutGroup("UI")] private TextMeshProUGUI _textTimer;
        [SerializeField, FoldoutGroup("UI")] private Image _progress;
        [SerializeField, FoldoutGroup("UI")] private RectTransform _rectImageJob;
        [SerializeField, FoldoutGroup("UI")] private RectTransform _rectPrice;
        [SerializeField, FoldoutGroup("UI")] private Image _buttonSell;
        [SerializeField, FoldoutGroup("UI")] private TextMeshProUGUI _textCurrentCount;
        [field: SerializeField, FoldoutGroup("UI")] public Button ButtonSell { get; private set; }
        [SerializeField, FoldoutGroup("Settings")] private IconConfig _iconConfig;
        [SerializeField, FoldoutGroup("Settings")] private Sprite _spriteButtonUnLock;
        [SerializeField, FoldoutGroup("Settings")] private Sprite _spriteButtonLock;
        [SerializeField, FoldoutGroup("Settings")] private float _delayClick;
        
        private PriceItem _item;
        private WaitForSeconds _delay;

        private void Awake()
        {
            _delay = new WaitForSeconds(_delayClick);
        }

        public void Init(PriceItem item)
        {
            _item = item;
            _iconInput.sprite = _iconConfig.GetIconItem(item.Input);
            foreach (var icon in _iconOutput)
            {
                icon.sprite = _iconConfig.GetIconItem(item.Output);
            }
            _textCost.text = $"{item.CountOutput}";
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectPrice);
            Canvas.ForceUpdateCanvases();
        }

        private void Start()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rectPrice);
            Canvas.ForceUpdateCanvases();
        }

        public void PerformNotify(ResourceItem data)
        {
            if (_item.Input != data.Type)
            {
                return;
            }
            _textCountInput.text = $"{data.Count}";
            _buttonSell.sprite = data.Count > 0 ? _spriteButtonUnLock : _spriteButtonLock;
            ButtonSell.interactable = data.Count > 0;
        }

        public void SetCurrentCount(int count)
        {
            _textCurrentCount.text = count > 0 ? $"{count}" : "";
        }
        
        public void StartAnimation()
        {
            _rectImageJob.Rotate(new Vector3(0f, 0f, Time.smoothDeltaTime * -100f));
        }
        
        public void SetProgress(float progress)
        {
            _progress.fillAmount = progress;
        }

        public void SetTotalTime(float time)
        {
            var timeSpan = TimeSpan.FromSeconds(time);
            _textTimer.text = $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
        }

        public void SetCountOutput(int count)
        {
            _textCountOutput.text = $"{count}";
        }
        
        public void StartClick()
        {
            StartCoroutine(WaitClick());
        }

        private IEnumerator WaitClick()
        {
            while (ButtonSell.interactable)
            {
                OnClickEvent?.Invoke();
                yield return _delay;
            }
            yield break;
        }

        public void StopClick()
        {
            StopAllCoroutines();
        }
    }
}
