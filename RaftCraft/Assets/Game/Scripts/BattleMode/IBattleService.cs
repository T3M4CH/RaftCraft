using System;

namespace Game.Scripts.BattleMode
{
    public interface IBattleService
    {
        public event Action<bool> OnChangeResult; 
        public event Action<BattleState> OnChangeState;
        public event Action OnSpawnerItem; 

        public BattleState CurrentState { get; }
    }
}