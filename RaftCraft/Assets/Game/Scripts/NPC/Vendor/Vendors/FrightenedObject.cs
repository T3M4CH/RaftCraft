using Game.Scripts.BattleMode;
using Reflex.Attributes;
using UnityEngine;

public class FrightenedObject : MonoBehaviour
{
    private IBattleService _battleService;

    [Inject]
    private void Construct(IBattleService battleService)
    {
        _battleService = battleService;
        _battleService.OnChangeState += ValidateState;
    }

    private void ValidateState(BattleState state)
    {
        if(IsBlock) return;
        
        gameObject.SetActive(state == BattleState.Idle);
    }

    private void OnDestroy()
    {
        _battleService.OnChangeState -= ValidateState;
    }
    
    public bool IsBlock { get; set; }
}
