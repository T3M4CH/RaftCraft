using System;
using System.Collections;
using Game.Scripts.BattleMode;
using Game.Scripts.InteractiveObjects;
using Game.Scripts.UI.WindowManager;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Prefabs.NPC.Vendors
{
    public abstract class BaseVendor : MonoBehaviour, IUiInteraction
    {
        public event Action<bool> OnInteraction = _ => { };
        public bool IsAbleEverywhere => false;
        public bool Interaction => !HaveOpenWindow;
        public InteractionType CurrentTypeInteraction => InteractionType.Npc;
        public float DelayAction => 0f;
        
        [SerializeField, FoldoutGroup("UI")] private GameObject icon;
        [SerializeField, FoldoutGroup("UI")] private Image _imageProgress;
        [SerializeField, FoldoutGroup("Settings")] private float _delayStay;
        private WindowManager _windowManager;
        protected IBattleService BattleService { get; set; }
        private float _progress;

        private bool HaveOpenWindow;
        
        protected void Initialize(IBattleService battleService)
        {
            BattleService = battleService;
            BattleService.OnChangeState += ValidateIcon;
        }

        protected virtual void Start()
        {
            HaveOpenWindow = false;
            _imageProgress.fillAmount = 0f;
        }
        
        public void Action()
        {
            _progress += Time.smoothDeltaTime;
            _imageProgress.fillAmount = _progress / _delayStay;
            if (_progress >= 1f && _windowManager != null)
            {
                _imageProgress.fillAmount = 0f;
                ShowWindow(_windowManager);
                HaveOpenWindow = true;
            }
        }

        public void EnterInteraction()
        {
            HaveOpenWindow = false;
            _progress = 0f;
            OnInteraction.Invoke(true);
            _imageProgress.fillAmount = 0f;
        }

        public void ExitInteraction()
        {
            HaveOpenWindow = false;
            _progress = 0f;
            OnInteraction.Invoke(false);
            _imageProgress.fillAmount = 0f;
            StopAllCoroutines();
        }

        public void OpenWindow(WindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        private void ValidateIcon(BattleState battleState)
        {
            switch (battleState)
            {
                case BattleState.Fight:
                    icon.gameObject.SetActive(false);
                    break;
                case BattleState.Idle:
                    icon.gameObject.SetActive(true);
                    break;
            }
        }
        
        private void OnDestroy()
        {
            OnInteraction = null;
            BattleService.OnChangeState -= ValidateIcon;
        }

        protected abstract void ShowWindow(WindowManager windowManager);
    }
}
