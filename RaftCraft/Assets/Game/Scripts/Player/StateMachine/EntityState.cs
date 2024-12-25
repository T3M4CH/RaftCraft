using Game.Scripts.Player.EntityGame;

namespace Game.Scripts.Player.StateMachine
{
    public abstract class EntityState
    {
        protected readonly EntityStateMachine stateMachine;
        public Entity Entity { get; private set; }
        public bool IsActive { get; private set; }
        public EntityState(EntityStateMachine stateMachine, Entity entity)
        {
            this.stateMachine = stateMachine;
            this.Entity = entity;
        }

        public virtual void Enter()
        {
            IsActive = true;
        }

        public virtual void Exit()
        {
            IsActive = false;
        }

        public virtual void Update()
        {
            
        }

        public virtual void FixedUpdate()
        {
        }
    }
}
