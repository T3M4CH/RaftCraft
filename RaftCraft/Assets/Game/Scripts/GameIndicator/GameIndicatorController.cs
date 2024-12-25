using System;
using System.Collections.Generic;
using Game.Scripts.BattleMode;
using Game.Scripts.Player;
using Game.Scripts.Player.Spawners;
using Sirenix.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Scripts.GameIndicator
{
    [System.Serializable]
    public struct IndicatorData
    {
        [field: SerializeField] public IndicatorObject Prefab { get; private set; }
        [field: SerializeField] public RectTransform Content { get; private set; }
        [field: SerializeField] public CanvasGroup CanvasWindow { get; private set; }
        [field: SerializeField] public bool Enabled { get; private set; }
    }

    public class GameIndicatorController : IControllerIndicator, IDisposable
    {
        private IndicatorData _data;
        private Dictionary<Transform, IndicatorObject> _indicators = new Dictionary<Transform, IndicatorObject>();
        private IPlayerService _player;
        private Transform _playerTransform;
        
        private readonly IBattleService _battleService;

        public GameIndicatorController(IndicatorData data, IPlayerService playerService, IBattleService battleService)
        {
            _data = data;
            _player = playerService;
            _battleService = battleService;
            _battleService.OnChangeState += ValidateBattleState;
            _player.AddListener(UpdatePlayer);
        }

        private void ValidateBattleState(BattleState state)
        {
            _data.CanvasWindow.alpha = state == BattleState.Idle ? 1 : 0;
        }

        private void UpdatePlayer(EPlayerStates state, EntityPlayer player)
        {
            if (!_data.Enabled) return;
            switch (state)
            {
                case EPlayerStates.SpawnPlayer:
                    _playerTransform = player.transform;
                    _indicators.ForEach(kv => kv.Value.SetPlayer(player.transform));
                    break;
                case EPlayerStates.PlayerInRaft:
                    _data.CanvasWindow.alpha = 1f;
                    break;
                case EPlayerStates.PlayerInWater:
                    _data.CanvasWindow.alpha = 1f;
                    break;
            }
        }

        public void AddTarget(Transform target, Vector3 position, float distanceThreshold, Sprite sprite, Color color, bool isStaticPosition = false, float multiplierOffset = 3f)
        {
            if (!_data.Enabled) return;
            if (_indicators.ContainsKey(target) == false)
            {
                var indicator = Object.Instantiate(_data.Prefab, _data.Content);
                indicator.SetTarget(target, position, distanceThreshold, sprite, color, isStaticPosition, multiplierOffset);
                if (_playerTransform)
                {
                    indicator.SetPlayer(_playerTransform);
                }

                _indicators.Add(target, indicator);
            }
        }

        public void RemoveTarget(Transform target)
        {
            if (_indicators.ContainsKey(target) == false)
            {
                return;
            }

            Object.Destroy(_indicators[target].gameObject);
            _indicators.Remove(target);
        }

        public void Dispose()
        {
            if (_player != null)
            {
                _player.RemoveListener(UpdatePlayer);
            }
            
            _battleService.OnChangeState += ValidateBattleState;
        }
    }
}