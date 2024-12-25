using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.InteractiveObjects.UI
{
    public class BuildCostView : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("UI")] private Image _imageProgress;
        [SerializeField, FoldoutGroup("UI")] private Image _iconImage;
        [SerializeField, FoldoutGroup("UI")] private TextMeshProUGUI _textCountResource;
        [SerializeField, FoldoutGroup("UI")] private RectTransform _panelEffect;
        [SerializeField, FoldoutGroup("UI")] private Image _backGround;
        [SerializeField, FoldoutGroup("UI")] private Image _progressTimer;
        [SerializeField, FoldoutGroup("UI")] private TextMeshProUGUI _textTimer;

        [SerializeField, FoldoutGroup("Panels")] private GameObject _panelPrice;
        [SerializeField, FoldoutGroup("Panels")] private GameObject _panelTimer;
        
        [SerializeField, FoldoutGroup("Feedback")]
        private float _duration;

        [SerializeField, FoldoutGroup("Feedback")]
        private Ease _ease;

        [SerializeField, FoldoutGroup("Feedback")]
        private Vector3 _sizePush;
        
        [SerializeField, FoldoutGroup("Feedback Not Item")]
        private float _durationNotItem;

        [SerializeField, FoldoutGroup("Feedback Not Item")]
        private Ease _easeNotItem;

        [SerializeField, FoldoutGroup("Feedback Not Item")]
        private Vector3 _force;

        [SerializeField, FoldoutGroup("Feedback Not Item")]
        private Color _colorNotIten;

        private Color _colorText;
        private Color _colorBackGround;

        private void Awake()
        {
            _colorText = _textCountResource.color;
            _colorBackGround = _backGround.color;
        }

        public void SetResourceSprite(Sprite sprite)
        {
            _iconImage.sprite = sprite;
        }

        public void SetStatePanelPrice(bool state)
        {
            _panelPrice.SetActive(state);
        }

        public void SetStatePanelTimer(bool state)
        {
            _panelTimer.SetActive(state);
        }

        public void SetProgressTimer(float progress)
        {
            _progressTimer.fillAmount = progress;
        }

        public void SetProgressTime(float time)
        {
            var timer = TimeSpan.FromSeconds(time);
            _textTimer.text = $"{timer.Minutes:00}:{timer.Seconds:00}";
        }

        public void SetProgress(float progress)
        {
            _imageProgress.fillAmount = progress;
        }

        public void SetCount(int count)
        {
            _textCountResource.text = $"{count}x";
        }

        [Button]
        public void PlayEffect()
        {
            _panelEffect.DOKill();
            _panelEffect.DOScale(_sizePush, _duration).SetEase(_ease).OnComplete(() =>
            {
                _panelEffect.DOScale(Vector3.one, _duration).SetEase(_ease);
            });
        }

        public void PlayEffectNotItems()
        {
            _panelEffect.DOKill();
            _panelEffect.DOShakePosition(_durationNotItem, _force).SetEase(_easeNotItem);
            _textCountResource.DOKill();
            _textCountResource.DOColor(Color.red, _durationNotItem).SetEase(_easeNotItem).SetLoops(2).OnComplete(() =>
            {
                _textCountResource.color = _colorText;
            });

            _backGround.DOKill();
            _backGround.DOColor(_colorNotIten, _durationNotItem).SetEase(_easeNotItem).SetLoops(2).OnComplete(() =>
            {
                _backGround.color = _colorBackGround;
            });
        }
    }
}
