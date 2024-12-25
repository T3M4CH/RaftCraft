using System;
using DG.Tweening;
using Game.Scripts.BattleMode;
using Game.Scripts.Player;
using Game.Scripts.Player.Spawners;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.DownDoor
{
    public class DownDoorObject : MonoBehaviour
    {
        enum StateDoor
        {
            Open,
            Close
        }
        
        [field: SerializeField] public Transform PointEnter { get; private set; }
        
        [SerializeField] private Transform _leftDoor;
        [SerializeField] private Transform _rightDoor;

        [SerializeField, FoldoutGroup("Feedback")] private float _duration;
        [SerializeField, FoldoutGroup("Feedback")] private Ease _ease;
        [SerializeField, FoldoutGroup("Settings")] private LayerMask _maskEnter;
        [SerializeField, FoldoutGroup("Settings")] private LayerMask _maskExit;

        private IBattleService _battleService;
        private IPlayerService _playerService;
        private StateDoor _state;

        public bool Interaction => _state == StateDoor.Open;
        
        [Inject]
        private void Construct(IBattleService battleService, IPlayerService playerService)
        {
            _battleService = battleService;
            _playerService = playerService;
            _battleService.OnChangeState += BattleServiceOnOnChangeState;
        }

        private void Start()
        {
            _playerService.AddListener(OnChangePlayerState);
        }

        private void OnChangePlayerState(EPlayerStates states, EntityPlayer player)
        {
            if(_battleService.CurrentState != BattleState.Idle) return;
            
            switch (states)
            {
                case EPlayerStates.NotPlayer:
                    break;
                case EPlayerStates.SpawnPlayer:
                    break;
                case EPlayerStates.PlayerInRaft:
                    Open();
                    break;
                case EPlayerStates.PlayerInWater:
                    Close();
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
                    throw new ArgumentOutOfRangeException(nameof(states), states, null);
            }
        }

        private void BattleServiceOnOnChangeState(BattleState state)
        {
            switch (state)
            {
                case BattleState.СutScene:
                    Close();
                    break;
                case BattleState.Fight:
                    Close();
                    break;
                case BattleState.Result:
                    Close();
                    break;
                case BattleState.Idle:
                    Open();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        public void Enter(EntityPlayer player)
        {
            player.Rb.excludeLayers = _maskEnter;
        }

        public void Exit(EntityPlayer player)
        {
            player.Rb.excludeLayers = _maskExit;
        }
        
        private void OnDestroy()
        {
            _playerService.RemoveListener(OnChangePlayerState);
            _battleService.OnChangeState -= BattleServiceOnOnChangeState;
        }


        [Button]
        private void Open()
        {
            _state = StateDoor.Open;
            _leftDoor.DOKill();
            _rightDoor.DOKill();
            _leftDoor.DORotate(new Vector3(0, 0f, -90f), _duration).SetEase(_ease);
            _rightDoor.DORotate(new Vector3(0, 0f, 90f), _duration).SetEase(_ease);
        }
        
        [Button]
        private void Close()
        {
            _state = StateDoor.Close;
            _leftDoor.DOKill();
            _rightDoor.DOKill();
            _leftDoor.DORotate(new Vector3(0, 0f, 0f), _duration).SetEase(_ease);
            _rightDoor.DORotate(new Vector3(0, 0f, 0f), _duration).SetEase(_ease);
        }
    }
}
