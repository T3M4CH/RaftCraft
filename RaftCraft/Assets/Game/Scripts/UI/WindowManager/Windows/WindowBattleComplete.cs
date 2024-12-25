using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.UI.WindowManager.Windows
{
    public class WindowBattleComplete : UIWindow
    {
        [SerializeField] private Transform _elementPanel;

        private Sequence _sequence;

        public override void Show()
        {
            _elementPanel.localScale = Vector3.zero;
            
            base.Show();
            
            _sequence = DOTween.Sequence()
                .Append(_elementPanel
                    .DOScale(Vector3.one, 1f)
                    .SetEase(Ease.OutBack))
                .AppendInterval(1.5f)
                .Append(_elementPanel
                    .DOScale(Vector3.zero, 0.6f)
                    .SetEase(Ease.InBack))
                .AppendCallback(Hide);
        }
        
        public override void Hide()
        {
            _sequence.Kill();
            _elementPanel.localScale = Vector3.zero;
            
            base.Hide();
        }
        
        private void OnDestroy()
        {
            _sequence.Kill();
        }
    }
}