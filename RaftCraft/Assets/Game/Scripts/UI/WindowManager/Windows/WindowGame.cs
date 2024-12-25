using System;
using DG.Tweening;
using Game.Scripts.Joystick.Extras;
using Game.Scripts.Quest;
using Game.Scripts.Quest.Interfaces;
using Game.Scripts.UI.Elements;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.WindowManager.Windows
{
    public class WindowGame : UIWindow
    {
        public event Action OnClickButtonBattle;
        
        [SerializeField] private ButtonBattle _fightButton;
        [SerializeField, FoldoutGroup("Feedback")] private float _delayOpen;
        [SerializeField, FoldoutGroup("Feedback")] private float _delayClose;
        [SerializeField, FoldoutGroup("Feedback")] private float _durationOpen;
        [SerializeField, FoldoutGroup("Feedback")] private float _durationClose;
        [SerializeField, FoldoutGroup("Feedback")] private Ease _easeOpen;
        [SerializeField, FoldoutGroup("Feedback")] private Ease _easeClose;
        [SerializeField, FoldoutGroup("Feedback button")] private Ease _easeButton;
        [SerializeField] private Transform _swordIcon;
        [SerializeField] private float _speedScaleIcon = 3f;
        
        private bool _playerControllerConfirm;
        private bool _questControllerConfirm;
        private IReadyService _readyService;
        private InputSingleton _inputSingleton;
        private bool _haveInteraction;
        private RectTransform _rectButton;

        [Inject]
        private void Init(InputSingleton inputSingleton, IReadyService readyService)
        {
            _inputSingleton = inputSingleton;
            Joystick.Setup(_inputSingleton);
            
            _readyService = readyService;
            
            _questControllerConfirm = false;
            
        }

        private void Awake()
        {
            _rectButton = _fightButton.GetComponent<RectTransform>();
        }

        private void Start()
        {
            _readyService.AddListener(TakeQuestControllerConfirm);
            _readyService.OnChangeTime += _fightButton.SetTimer;
            _swordIcon.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), _speedScaleIcon, 1).SetLoops(-1).SetEase(_easeButton);
        }

        private void Update()
        {
            //_swordIcon.localScale = Vector3.one * Mathf.Lerp(1.1f, 0.9f, Mathf.PingPong(Time.time * _speedScaleIcon, 1f));
        }

        private void OnEnable()
        {
            _fightButton.OnClick += ClickBattle;
        }

        private void OnDisable()
        {
            _fightButton.OnClick -= ClickBattle;
        }

        private void ClickBattle()
        {
            OnClickButtonBattle?.Invoke();
        }
        
        private void TakeQuestControllerConfirm(ReadyState readyState)
        {
            _fightButton.SetState(readyState);
        }

        public void OpenSettings()
        {
            WindowManager.OpenWindow<WindowSettings>();
        }

        private void OnDestroy()
        {
            _readyService.OnChangeTime -= _fightButton.SetTimer;
            _readyService.RemoveListener(TakeQuestControllerConfirm);
        }

        public RectTransform FightButton => _rectButton;
        
        [field: SerializeField] public MonoJoystick Joystick { get; private set; }
    }
}