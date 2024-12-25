using Game.Scripts.Player.EntityGame;
using Game.Scripts.Player.StateMachine;
using UnityEngine;

namespace Game.Scripts.NPC.Fish.FishStates
{
    public abstract class FishState : EntityState
    {
        protected BaseFish currentBaseFish => Entity as BaseFish;
        protected Animator CurrentAnimator => currentBaseFish.ComponentsFish.Animator[currentBaseFish.Level];
        
        protected readonly int HashSpeedRun = Animator.StringToHash("SpeedRun");
        protected readonly int RunHash = Animator.StringToHash("Run");
        protected FishState(EntityStateMachine stateMachine, Entity entity) : base(stateMachine, entity)
        {
            
        }
    }
}
