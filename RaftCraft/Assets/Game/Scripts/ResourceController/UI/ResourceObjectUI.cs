using System;
using DG.Tweening;
using Game.Scripts.ResourceController.Enums;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game.Scripts.ResourceController.UI
{
    public class ResourceObjectUI : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("UI")] private CanvasGroup _canvasGroup;
        [SerializeField, FoldoutGroup("UI")] private TextMeshProUGUI _textCountResource;
        [SerializeField, FoldoutGroup("UI")] private Image _imageIconResource;

        [SerializeField, FoldoutGroup("Settings")] private IconConfig _config;

        [SerializeField, FoldoutGroup("Feedback")]
        private float _durationMoveUp;

        [SerializeField, FoldoutGroup("Feedback")]
        private float _durationMoveTarget;

        [SerializeField, FoldoutGroup("Feedback")]
        private Ease _easeMoveUp;
        
        [SerializeField, FoldoutGroup("Feedback")]
        private Ease _easeFly;

        [SerializeField, FoldoutGroup("Feedback")]
        private Ease _easeMoveTarget;

        [SerializeField, FoldoutGroup("Feedback")]
        private Vector3 _offsetMoveUp;

        private Sequence _sequence;

        public void Show(EResourceType type, int count, Vector3 positionShow)
        {
            SetUI(type, count);
            transform.position = positionShow;
            MoveUp();
        }

        public void ShowToUi(EResourceType type, int count, Vector3 positionShow, RectTransform targetPosition)
        {
            SetUI(type, count);
            transform.position = positionShow;
            MoveToUi(targetPosition);
        }

        private void MoveToUi(RectTransform target)
        {
            transform.DOKill();
            _sequence.Kill();
            _sequence = DOTween.Sequence();
            
            _textCountResource.text = string.Empty;

            _sequence.AppendInterval(Random.Range(0.1f, 0.2f));
            _sequence.Append(transform.DOMove(transform.position + new Vector3(Random.Range(-200f, 200f),Random.Range(200f,300f),0), _durationMoveUp).SetEase(_easeFly));
            _sequence.Append(transform.DOMove(target.position, _durationMoveUp).SetEase(_easeMoveUp));
            _sequence.OnComplete(() => { gameObject.SetActive(false); });
        }

        [Button]
        private void MoveUp()
        {
            transform.DOKill();
            transform.DOMove(transform.position + _offsetMoveUp, _durationMoveUp).SetEase(_easeMoveUp);
            _canvasGroup.DOFade(0f, _durationMoveUp).SetEase(_easeMoveUp).OnComplete(() => { gameObject.SetActive(false); });
        }

        private void SetUI(EResourceType type, int count)
        {
            _imageIconResource.sprite = _config.GetIconItem(type);
            _textCountResource.text = $"+{count}";
            _canvasGroup.alpha = 1f;
        }

        private void OnDestroy()
        {
            transform.DOKill();
            _sequence.Kill();
            _canvasGroup.DOKill();
        }
    }
}