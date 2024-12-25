using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Game.Scripts.DamageEffector
{
    public class DamageText : MonoBehaviour
    {
        [field: SerializeField, FoldoutGroup("UI")] public RectTransform Rect { get; private set; }
        [SerializeField, FoldoutGroup("UI")] private TextMeshProUGUI _text;
        [SerializeField, FoldoutGroup("UI")] private CanvasGroup _group;
        [SerializeField, FoldoutGroup("Settings")] private Color _colorDefault;
        [SerializeField, FoldoutGroup("Settings")] private Color _colorCrit;
        [SerializeField, FoldoutGroup("Settings")] private float _sizeDefault;
        [SerializeField, FoldoutGroup("Settings")] private float _sizeCrit;

        [SerializeField, FoldoutGroup("Feedback")] private Vector3 _directionMove;
        [SerializeField, FoldoutGroup("Feedback")] private Ease _easeMove;
        [SerializeField, FoldoutGroup("Feedback")] private float _durationMove;
        
        public void SetCritical(bool critical = false)
        {
            _text.color = critical ? _colorCrit : _colorDefault;
            _text.fontSize = critical ? _sizeCrit : _sizeDefault;
        }
        
        public void SetValue(int damage)
        {
            _text.text = $"-{damage}";
        }

        public void SetValue(float damage)
        {
            _text.text = $"-{damage:0}";
        }

        public void StartMove()
        {
            _group.DOKill();
            Rect.DOKill();
            _group.alpha = 0f;
            _group.DOFade(1f, 0.1f).SetEase(_easeMove).SetDelay(Random.Range(0f, 0.5f)).OnComplete(() =>
            {
                _group.DOFade(0f, _durationMove).SetEase(_easeMove);
            });
            Rect.DOMove(Rect.position + DirectionMove(), _durationMove).SetEase(_easeMove).OnComplete(
                () =>
                {   
                    gameObject.SetActive(false);
                });
        }

        private Vector3 DirectionMove()
        {
            var result = _directionMove;
            result.y += Random.Range(0f, 30f);
            return result;
        }

        private void OnDisable()
        {
            _group.DOKill();
            Rect.DOKill();
        }
    }
}
