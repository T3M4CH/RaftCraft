using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.WindowManager.Windows
{
    public class WindowBattleFail : UIWindow
    {
        [SerializeField] private Button _button;
        [SerializeField] private Transform _elementPanel;

        public event Action OnClickContinue;

        public override void Initialize(WindowManager windowManager)
        {
            base.Initialize(windowManager);
            
            _button.onClick.AddListener(OnClickButton);
        }

        private void OnClickButton()
        {
            WindowManager.CloseWindow<WindowBattleFail>();
            OnClickContinue?.Invoke();
        }

        public override void Show()
        {
            base.Show();

            _elementPanel
                .DOScale(Vector3.one, 0.6f)
                .From(0f)
                .SetEase(Ease.OutBack);
        }

        public override void Hide()
        {
            base.Hide();

            _elementPanel.DOKill();
        }
        
        private void OnDestroy()
        {
            _elementPanel.DOKill();
            _button.onClick.RemoveAllListeners();
        }
    }
}