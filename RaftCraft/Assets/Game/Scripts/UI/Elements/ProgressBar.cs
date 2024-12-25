using System;
using UnityEngine;

namespace Game.Scripts.UI.Elements
{
    public class ProgressBar : MonoBehaviour
    {
        [Header("Bar settings")]
        [SerializeField] private Vector2 _positionMinMax = new Vector2(-168f, 0f);
        [SerializeField] private Vector2 _scaleMinMax = new Vector2(25f, 360f);
        [SerializeField] private float _maxSpeedFill = 0.5f;
        [SerializeField] private float _minSpeedFill = 0.03f;
        [SerializeField] private RectTransform _rectTransform;

        private float _realValue;
        private float _currentValue;
        private float _initialValue;

        public void ChangeValue(float value, bool smooth)
        {
            _initialValue = _currentValue;
            _currentValue = value;

            if (smooth) return;

            _realValue = value;
            _rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(_positionMinMax.x, _positionMinMax.y, _realValue), 0f);
            _rectTransform.sizeDelta = new Vector2(Mathf.Lerp(_scaleMinMax.x, _scaleMinMax.y, _realValue), 22f);
        }

        private void Update()
        {
            if (Math.Abs(_realValue - _currentValue) > 0.01f)
            {
                var t = Mathf.InverseLerp(_initialValue, _currentValue, _realValue);
                var step = Time.smoothDeltaTime * Mathf.Lerp(_maxSpeedFill, _minSpeedFill, t);

                _realValue =
                    Mathf.MoveTowards(_realValue, _currentValue, step);
                
                _rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(_positionMinMax.x, _positionMinMax.y, _realValue), 0f);
                _rectTransform.sizeDelta = new Vector2(Mathf.Lerp(_scaleMinMax.x, _scaleMinMax.y, _realValue), 22f);
            }
        }
    }
}