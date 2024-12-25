using System;
using Game.Scripts.Player.EntityGame;
using Game.Scripts.Player.Spawners;
using UnityEngine;

public class BattleStatistics
{
    private readonly IPlayerService _playerService;

    private int _totalCountHuman;
    private int _countDestroyHuman;

    public event Action<bool> OnPlayerWin;
    public event Action<float> OnUpdateProgress;
    
    public BattleStatistics(IPlayerService playerService)
    {
        _playerService = playerService;
        
        _playerService.AddListener(OnDeathStatePlayer);
    }

    public void Setup(int countTotalHuman)
    {
        _countDestroyHuman = 0;
        _totalCountHuman = countTotalHuman;
    }
    
    public void UpdateProgress()
    {
        _countDestroyHuman++;
        OnUpdateProgress?.Invoke(Mathf.InverseLerp(0, _totalCountHuman, _countDestroyHuman));
    }
    
    public void DestructionCheck()
    {
        if (_countDestroyHuman >= _totalCountHuman)
        {
            OnPlayerWin?.Invoke(true);
        }
    }
    
    private void OnDeathStatePlayer(EPlayerStates state, Entity entity)
    {
        if (state == EPlayerStates.PlayerDeadInRaft)
            OnPlayerWin?.Invoke(false);
    }

    public void Destroy()
    {
        _playerService.RemoveListener(OnDeathStatePlayer);
    }
}