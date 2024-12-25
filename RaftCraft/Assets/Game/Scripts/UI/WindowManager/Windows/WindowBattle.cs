using System;
using DG.Tweening;
using Game.Scripts.BattleMode;
using Game.Scripts.Days;
using Game.Scripts.UI.Elements;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.WindowManager.Windows
{
    public class WindowBattle : UIWindow
    {
        [SerializeField] private float _delay = 0.5f;
        [SerializeField] private float _duration = 0.3f;
        [SerializeField] private bool _smoothBar = true;
        [SerializeField] private ProgressBar _progressBar;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _dayFirst, _daySecond;
        [SerializeField] private Button _buttonEndDay;
        
        private float _progress;
        private IBattleService _battleService;
        private IDayService _dayService;

        public event Action OnClickEndDay;

        [Inject]
        private void Construct(IDayService dayService)
        {
            _dayService = dayService;

            _dayService.OnDayStart += UpdateDayInfo;
        }

        public override void Initialize(WindowManager windowManager)
        {
            base.Initialize(windowManager);
            
            _buttonEndDay.onClick.AddListener(ClickEndDay);
        }

        private void ClickEndDay()
        {
            OnClickEndDay?.Invoke();
            _buttonEndDay.transform.DOKill();
            _buttonEndDay.gameObject.SetActive(false);
        }
        
        public void ChangeValueProgressBar(float value)
        {
            _progressBar.ChangeValue(value, _smoothBar);
        }

        private void UpdateDayInfo(int dayNumber)
        {
            _dayFirst.SetText($"{dayNumber}");
            _daySecond.SetText($"{dayNumber + 1}");
        }

        public void ShowEndDayButton()
        {
            _buttonEndDay.transform
                .DOScale(Vector3.one * 1.1f, 0.7f)
                .SetLoops(-1, LoopType.Yoyo);
            
            _buttonEndDay.gameObject.SetActive(true);
        }
        
        public override void Show()
        {
            base.Show();
            
            _buttonEndDay.gameObject.SetActive(false);
            
            _progressBar.ChangeValue(0f, false);
            _canvasGroup
                .DOFade(1f, _duration)
                .SetDelay(_delay)
                .From(0f);
        }
        
        public override void Hide()
        {
            _canvasGroup
                .DOFade(0f, _duration)
                .From(1f)
                .OnComplete(() => base.Hide());
        }

        private void OnDestroy()
        {
            _buttonEndDay.onClick.RemoveAllListeners();
            _dayService.OnDayStart -= UpdateDayInfo;

            _buttonEndDay.transform.DOKill();
            _canvasGroup.DOKill();
        }
    }
}