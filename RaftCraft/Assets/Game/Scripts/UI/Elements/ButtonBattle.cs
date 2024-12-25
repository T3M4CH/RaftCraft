using System;
using Game.Scripts.Player;
using Game.Scripts.Player.Spawners;
using Game.Scripts.Quest;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI.Elements
{
    public class ButtonBattle : MonoBehaviour
    {
        public event Action OnClick;
        
        [SerializeField, FoldoutGroup("UI")] private CanvasGroup _group;
        [SerializeField, FoldoutGroup("UI")] private GameObject _ray;
        [SerializeField, FoldoutGroup("UI")] private TextMeshProUGUI _textTimer;

        private ReadyState _state;
        private ReadyState _tempState;
        private ReadyState State
        {
            get => _state;
            set
            {
                _state = value;
                switch (_state)
                {
                    case ReadyState.Disabled:
                        _textTimer.enabled = false;
                        _group.alpha = 0f;
                        break;
                    case ReadyState.Timer:
                        _textTimer.enabled = true;
                        _group.alpha = 0.25f;
                        break;
                    case ReadyState.Enabled:
                        _textTimer.enabled = false;
                        _group.alpha = 1f;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private IPlayerService _playerService;
        
        [Inject]
        private void Construct(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        private void Start()
        {
            _playerService.AddListener(OnChangePlayer);
        }

        private void OnChangePlayer(EPlayerStates state, EntityPlayer player)
        {
            switch (state)
            {
                case EPlayerStates.NotPlayer:
                    break;
                case EPlayerStates.SpawnPlayer:
                    break;
                case EPlayerStates.PlayerInRaft:
                    State = _tempState;
                    break;
                case EPlayerStates.PlayerInWater:
                    State = ReadyState.Disabled;
                    break;
                case EPlayerStates.PlayerInBattle:
                    break;
                case EPlayerStates.PlayerDead:
                    break;
                case EPlayerStates.PlayerDeadInRaft:
                    break;
                case EPlayerStates.PlayerDeadInWater:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void OnDestroy()
        {
            _playerService.RemoveListener(OnChangePlayer);
        }

        public void SetTimer(float second)
        {
            if (second == 0f)
            {
                return;
            }
            var time = TimeSpan.FromSeconds(second);
            _textTimer.text = $"{time.Minutes:00}:{time.Seconds:00}";
        }

        public void SetState(ReadyState state)
        {
            State = state;
            _tempState = state;
        }
        
        public void Click()
        {
            if (State != ReadyState.Enabled)
            {
                return;
            }
            OnClick?.Invoke();
        }
        
    }
}
