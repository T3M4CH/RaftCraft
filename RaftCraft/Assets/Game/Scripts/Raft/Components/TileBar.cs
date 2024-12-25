using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Raft.Components
{
    public class TileBar : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("UI")] private CanvasGroup _canvasGroup;
        [SerializeField, FoldoutGroup("UI")] private CanvasGroup _groupProgress;
        [SerializeField, FoldoutGroup("UI")] private Image _progress;

        [SerializeField, FoldoutGroup("Feedback")]
        private float _durationOpen;

        [SerializeField, FoldoutGroup("Feedback")]
        private Ease _easeOpen;

        private void Awake()
        {
            _canvasGroup.alpha = 0f;
            _groupProgress.alpha = 0f;
        }

        public void Show()
        {
            _canvasGroup.DOKill();
            _canvasGroup.DOFade(1f, _durationOpen).SetEase(_easeOpen);
        }

        public void ShowProgress()
        {
            _groupProgress.DOKill();
            _groupProgress.DOFade(1f, _durationOpen).SetEase(_easeOpen);
        }

        public void SetProgress(float progress)
        {
            _progress.fillAmount = progress;
        }

        public void HideProgress()
        {
            _groupProgress.DOKill();
            _groupProgress.DOFade(0f, _durationOpen).SetEase(_easeOpen);
        }

        public void Hide()
        {
            _canvasGroup.DOKill();
            _canvasGroup.DOFade(0f, _durationOpen).SetEase(_easeOpen);
        }

        private void OnDisable()
        {
            _canvasGroup.DOKill();
        }
    }
}
