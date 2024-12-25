using System;
using System.Linq;
using Game.Scripts.BattleMode;
using Game.Scripts.Days;
using Game.Scripts.GameIndicator;
using Game.Scripts.Joystick.Interfaces;
using Game.Scripts.Player.Spawners;
using Game.Scripts.Quest.Interfaces;
using Game.Scripts.Raft.Interface;
using Game.Scripts.ResourceController.Interfaces;
using Game.Scripts.Saves;
using Game.Scripts.UI.WindowManager;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Scripts.Quest
{
    public class QuestManager : IQuestService, IDisposable, IReadyClient
    {
        public QuestManager
        (
            IDayService dayController,
            GameSave gameSave,
            IPlayerService playerSpawner,
            WindowManager windowManager,
            IGameResourceService gameResourceService,
            IRaftStructures raftStructures,
            IControllerIndicator controllerIndicator,
            IResourceService resourceService,
            IBattleService battleService
        )
        {
            _dayController = dayController;
            if (_dayController.CurrentDay == 0) return;

            _questsConfig = Resources.Load<QuestsConfig>("QuestsPerDay");

            if (_questsConfig == null)
            {
                throw new Exception("Quest config wasn't found");
            }

            _gameSave = gameSave;
            _playerSpawner = playerSpawner;
            _windowManager = windowManager;
            _gameResourceService = gameResourceService;
            _raftStructures = raftStructures;
            _controllerIndicator = controllerIndicator;
            _resourceService = resourceService;
            _battleService = battleService;
            _dayController.OnDayStart += ValidateDay;
        }

        public event Action<bool> OnStatusChanged;

        private MonoTutorialBase _currentTutorial;

        private readonly QuestsConfig _questsConfig;
        private readonly IDayService _dayController;
        private readonly GameSave _gameSave;
        private readonly IPlayerService _playerSpawner;
        private readonly IJoystickService _joystickService;
        private readonly WindowManager _windowManager;
        private readonly IGameResourceService _gameResourceService;
        private readonly IRaftStructures _raftStructures;
        private readonly IControllerIndicator _controllerIndicator;
        private readonly IResourceService _resourceService;
        private readonly IBattleService _battleService;
        private bool _ready;
        public event Action<IReadyClient> OnChangeReady;
        public bool IsReady => _ready;
        public float Delay => 0f;

        public void SetButtonReady(bool value)
        {
            _ready = value;
            OnChangeReady?.Invoke(this);
        }

        private void ValidateDay(int id)
        {
            var questPair = _questsConfig.QuestPerDays.FirstOrDefault(quest => quest.DayId == id);
            if (!questPair.Equals(default))
            {
                _currentTutorial = Object.Instantiate(questPair.QuestPrefab);

                var extraParam = questPair.DayId switch
                {
                    1 => new {_gameResourceService, _battleService, _windowManager},
                    3 => _resourceService,
                    6 => _resourceService,
                    10 => new { _raftStructures, _controllerIndicator, _resourceService },
                    _ => (object)questPair.ExtraParams
                };

                _currentTutorial.OnComplete += PerformTutorialComplete;
                _currentTutorial.OnChangeButton += SetButtonReady;
                _currentTutorial.Initialize(_gameSave, _playerSpawner, _windowManager, extraParam);
            
                return;
            }

            SetButtonReady(true);
        }

        private void PerformTutorialComplete()
        {
            _currentTutorial.OnComplete -= PerformTutorialComplete;
            _currentTutorial.OnChangeButton -= SetButtonReady;
            _currentTutorial = null;

            SetButtonReady(true);
        }

        public void Dispose()
        {
            if (_currentTutorial)
            {
                _currentTutorial.OnComplete -= PerformTutorialComplete;
                _currentTutorial.OnChangeButton -= SetButtonReady;
            }

            _dayController.OnDayStart -= ValidateDay;

            OnStatusChanged = null;
        }

        
    }
}