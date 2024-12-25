using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.NPC.Fish.Systems
{
    public class FishBar : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("UI")] private TextMeshProUGUI _textLevel;
        [SerializeField, FoldoutGroup("UI")] private Image _imageProgress;
        [SerializeField, FoldoutGroup("UI")] private Image _imageStateLock;
        [SerializeField, FoldoutGroup("UI")] private Image _imageLock;
        [SerializeField, FoldoutGroup("UI")] private GameObject _contentLevel;
        [SerializeField, FoldoutGroup("UI")] private GameObject _contentProgress;

        [SerializeField, FoldoutGroup("Settings")]
        private Color _spriteLock;

        [SerializeField, FoldoutGroup("Settings")]
        private Color _spriteUnLock;

        private FishLockState _state;

        public void SetStateLock(FishLockState state)
        {
            _state = state;
            _imageLock.enabled = false;
            
            switch (state)
            {
                case FishLockState.Lock:
                    _imageStateLock.color = _spriteLock;
                    //_imageLock.enabled = true;
                    SetStateProgressBar(false);
                    break;
                case FishLockState.UnLock:
                    _imageStateLock.color = _spriteUnLock;
                    SetStateProgressBar(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public void TryShowLevelBar()
        {
            var state = _state == FishLockState.Lock;
            SetStateLevelBar(state);
            SetLockSprite(state);
        }

        public void SetStateLevelBar(bool state)
        {
            _contentLevel.SetActive(state);

            if (!state)
            {
                SetLockSprite(false);
            }
        }

        public void SetLockSprite(bool value)
        {
            _imageLock.enabled = value;
        }

        public void SetStateProgressBar(bool state)
        {
            _contentProgress.SetActive(state);
        }

        public void SetLevel(int level)
        {
            _textLevel.text = $"{level}";
        }

        public void SetProgress(float progress)
        {
            _imageProgress.fillAmount = progress;
        }
    }
}