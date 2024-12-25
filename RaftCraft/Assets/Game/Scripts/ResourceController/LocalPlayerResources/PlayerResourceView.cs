using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game.Scripts.ResourceController.LocalPlayerResources
{
    public class PlayerResourceView : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("UI")] private CanvasGroup _group;
        [SerializeField, FoldoutGroup("UI")] private TextMeshProUGUI _textCountResource;

        [SerializeField, FoldoutGroup("Feedback")]
        private float _durationShow;

        [SerializeField, FoldoutGroup("Feedback")]
        private Ease _easeShow;

        [SerializeField, FoldoutGroup("Feedback")]
        private float _durationFail;

        [SerializeField, FoldoutGroup("Feedback")]
        private Ease _easeFail;

        [SerializeField, FoldoutGroup("Feedback")]
        private Color _colorFail;

        private Color _colorDefault;

        private void Awake()
        {
            _colorDefault = _textCountResource.color;
        }

        public void SetCount(int current, int max)
        {
            _textCountResource.text = $"{current}/{max}";
            if (current == max)
            {
                _textCountResource.text = "MAX";
            }
            Canvas.ForceUpdateCanvases();
        }

        private void Start()
        {
            Hide();
        }


        public void Show()
        {
            _group.DOKill();
            _group.DOFade(1f, _durationShow).SetEase(_easeShow);
        }

        public void PlayFail()
        {
            _textCountResource.DOKill();
            _textCountResource.DOColor(_colorFail, _durationFail).SetEase(_easeFail).SetLoops(2).OnComplete(() =>
            {
                _textCountResource.DOColor(_colorDefault, _durationFail).SetEase(_easeFail);
            });
        }


        public void Hide()
        {
            _group.DOKill();
            _group.DOFade(0f, _durationShow).SetEase(_easeShow);
        }

        private void OnDestroy()
        {
            _group.DOKill();
        }
    }
}
