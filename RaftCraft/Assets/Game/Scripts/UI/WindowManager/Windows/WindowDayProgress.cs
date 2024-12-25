using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.UI.WindowManager.Windows
{
    public class WindowDayProgress : UIWindow
    {
        [SerializeField] private float _delay = 0.5f;
        [SerializeField] private float _duration = 0.3f;
        [SerializeField] private CanvasGroup _canvasGroup;

        private Tween _tween;
        
        public override void Show()
        {
            _tween.Kill();
            _tween = _canvasGroup
                .DOFade(1f, _duration)
                .SetDelay(_delay)
                .From(0f);
        }
        
        public override void Hide()
        {
            _tween.Kill();
            _tween = _canvasGroup
                .DOFade(0f, _duration)
                .From(1f);
        }

        private void OnDestroy()
        {
            _tween.Kill();
        }
    }
}
