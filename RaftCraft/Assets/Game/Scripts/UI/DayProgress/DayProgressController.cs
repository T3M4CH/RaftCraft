using Game.Scripts.BattleMode;
using Game.Scripts.Days;
using Game.Scripts.Player;
using Game.Scripts.Player.Spawners;
using Game.Scripts.UI.DayProgress.Enums;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI.DayProgress
{
    public class DayProgressController : MonoBehaviour
    {
        [SerializeField] private int _startOffset;
        [SerializeField] private int _columnCount;
        [SerializeField] private Transform _columnsParent;
        [SerializeField] private ColumnDayInfo _columnDayInfo;
        [SerializeField] private TMP_Text _levelText;

        [SerializeField] private CanvasGroup _canvasGroup;
        
        //TODO: Удалить, когда будут уровни
        [SerializeField] private SerializableLevelStruct[] _levelStructs;

        private int _currentDay;
        private int _offset;
        private BiomesSceneManager _biomesSceneManager;
        private IDayService _dayController;

        private ColumnDayInfo[] _dayInfo;
        //private SerializableLevelStruct[] _levelStructs;

        private IPlayerService _player;
        private IBattleService _battleService;
        [Inject]
        private void Construct(IDayService dayController, IPlayerService service, IBattleService battleService)
        {
            if (enabled == false) return;
            _battleService = battleService;
            _player = service;
            _player.AddListener(UpdatePlayerState);
            _currentDay = dayController.CurrentDay;

            SpawnInfos();

            _dayController = dayController;
            _dayController.OnDayStart += UpdateDay;

            Spawn();
        }
        
        private void OnValidate()
        {
            for (var i = 0; i < _levelStructs.Length; i++)
            {
                ref var x = ref _levelStructs[i];
                x.DayId = i + 1;
            }
        }

        private void UpdatePlayerState(EPlayerStates state, EntityPlayer player)
        {
            if(_dayController.CurrentDay <= 0) return;
            
            switch (state)
            {
                case EPlayerStates.PlayerInRaft:
                    if (_battleService.CurrentState == BattleState.Idle)
                    {
                        _canvasGroup.alpha = 1f;
                    }
                    break;
                case EPlayerStates.PlayerInWater:
                    _canvasGroup.alpha = 0f;
                    break;
            }
        }

        private void UpdateDay(int id)
        {
            _currentDay = id;
            Spawn();
        }

#if UNITY_EDITOR
        [Button]
        private void Right()
        {
            _currentDay += 1;

            _dayController.CompleteDay();
            Spawn();
        }

        [Button]
        private void Left()
        {
            _currentDay -= 1;

            Spawn();
        }
#endif

        private void Spawn()
        {
            _levelText.text = $"Day {_currentDay}";

            var dayId = 0;
            if (_currentDay < _startOffset)
            {
                for (int i = 1; i <= _columnCount; i++)
                {
                    SetColumn(i, _dayInfo[dayId++]);
                }
            }
            else
            {
                for (int i = _currentDay - _startOffset + 1; i <= _currentDay + (_columnCount - _startOffset); i++)
                {
                    SetColumn(i, _dayInfo[dayId++]);
                }
            }
        }

        private void SetColumn(int i, ColumnDayInfo dayInfo)
        {
            dayInfo.SetColumn(_levelStructs.Length >= i ? _levelStructs[i - 1].DayType : EDayType.Default);

            if (_currentDay > i)
            {
                dayInfo.SetColor(EDayCompleteType.Complete);
            }
            else if (_currentDay == i)
            {
                dayInfo.SetColor(EDayCompleteType.InProgress);
            }
            else
            {
                dayInfo.SetColor(EDayCompleteType.NotStarted);
            }
        }

        private void SpawnInfos()
        {
            _dayInfo = new ColumnDayInfo[_columnCount];

            for (var i = 0; i < _dayInfo.Length; i++)
            {
                _dayInfo[i] = Instantiate(_columnDayInfo, _columnsParent);
            }
        }

        private void OnDestroy()
        {
            _dayController.OnDayStart -= UpdateDay;
            
            if (_player != null)
            {
                _player.RemoveListener(UpdatePlayerState);
            }
        }
    }
}