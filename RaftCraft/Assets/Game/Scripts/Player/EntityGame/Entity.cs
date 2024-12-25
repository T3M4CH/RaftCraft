using Game.Scripts.NPC;
using Game.Scripts.Player.PlayerController.PlayerStates;
using Game.Scripts.Player.StateMachine;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Player.EntityGame
{
    public abstract class Entity : MonoBehaviour
    {
        [field: SerializeField, FoldoutGroup("Entity Settings")] public EntityType Type { get; private set; }
        [field: SerializeField, FoldoutGroup("Entity Components")] public EntityMonoComponents Components { get; private set; }
        protected EntityStateMachine _stateMachine;

        public EntityStateMachine StateMachine => _stateMachine;

        public abstract bool HaveLife { get; }
        
        public virtual LocationSpace Space => LocationSpace.Ground;

        public virtual void InitState(EntityStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        protected virtual void Update()
        {
            if (_stateMachine != null)
            {
                _stateMachine.Update();
            }
        }
        
        protected virtual void FixedUpdate()
        {
            if (_stateMachine != null)
            {
                _stateMachine.FixedUpdate();
            }
        }

        protected virtual void OnDestroy()
        {
            if (_stateMachine != null)
            {
                _stateMachine.Disable();
            }
        }
    }
}
