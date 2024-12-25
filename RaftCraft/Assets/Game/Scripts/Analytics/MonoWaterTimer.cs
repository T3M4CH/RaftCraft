using System;
using Game.Scripts.Days;
using Game.Scripts.Player;
using Game.Scripts.Player.Spawners;
using GTap.Analytics;
using Reflex.Attributes;
using UnityEngine;

public class MonoWaterTimer : MonoBehaviour
{
    private float _timer;
    private bool _inWater;
    private IPlayerService _playerSpawner;
    private IDayService _dayController;
    
    [Inject]
    private void Construct(IPlayerService playerSpawner, IDayService dayController)
    {
        _playerSpawner = playerSpawner;
        _playerSpawner.AddListener(ValidatePlayerState);
        _dayController = dayController;
        _dayController.OnDayStart += ValidateDay;
    }

    private void ValidateDay(int dayId)
    {
        if(dayId == 1) return;
        
        GtapAnalytics.TimeSpentInWater(dayId - 1, _dayController.AttemptsCount, _timer);
        
        _timer = 0;
    }

    private void ValidatePlayerState(EPlayerStates state, EntityPlayer player)
    {
        switch (state)
        {
            case EPlayerStates.NotPlayer:
                break;
            case EPlayerStates.SpawnPlayer:
                break;
            case EPlayerStates.PlayerInRaft:
                _inWater = false;
                break;
            case EPlayerStates.PlayerInWater:
                _inWater = true;        
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

    private void Update()
    {
        if (_inWater)
        {
            _timer += Time.deltaTime;
        }
    }
}
