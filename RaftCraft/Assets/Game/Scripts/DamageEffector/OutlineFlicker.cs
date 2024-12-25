using System.Collections;
using UnityEngine;

namespace Game.Scripts.DamageEffector
{
    public class OutlineFlicker : MonoBehaviour
    {
        [SerializeField] private Outline _outline;
        [SerializeField] private Color _colorFlick;
        [SerializeField] private int _countFlick;
        [SerializeField] private float _durationFlick;

        private WaitForSeconds _duration;

        private Coroutine _coroutine;

        [SerializeField]
        private Color _defaultColor;
        
        private void Awake()
        {
            _duration = new WaitForSeconds(_durationFlick);
            _defaultColor = _outline.OutlineColor;
        }

        public void PlayEffect()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = StartCoroutine(WaitEffect());
            }
            else
            {
                _coroutine = StartCoroutine(WaitEffect());
            }
        }

        private IEnumerator WaitEffect()
        {
            for (var i = 0; i < _countFlick; i++)
            {
                _outline.OutlineColor = _colorFlick;
                yield return _duration;
                _outline.OutlineColor = _defaultColor;
            }
        }
    }
}
