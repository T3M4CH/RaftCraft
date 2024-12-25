using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.DamageEffector
{
    public class DamageFeedback : MonoBehaviour
    {
        [SerializeField] private float _durationTween;
        [SerializeField] private AnimationCurve _animationCurve;
        [SerializeField] private int _indexMaterial = 0;
        [SerializeField] private Color _colorDamage = Color.white;
    
        private Tween _tween;
        private Renderer _renderer;
    
        private readonly int _emissionColorHash = Shader.PropertyToID("_EmissionColor");

        public int IndexMaterial => _indexMaterial;

        public void SetRenderer(Renderer renderer)
        {
            _renderer = renderer;
        }
    
        [Button]
        public void TweenExecute()
        {
            _tween.Kill();
            _tween = _renderer.materials[_indexMaterial]
                .DOColor(_colorDamage, _emissionColorHash, _durationTween)
                .SetEase(_animationCurve)
                .From(Color.black);
        }
    }
}