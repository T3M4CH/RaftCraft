using Game.Scripts.Player.EntityGame;
using Game.Scripts.Player.StateMachine;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Scripts.Player.PlayerController.PlayerStates
{
    public class PlayerDeathInGround : EntityState
    {
        private readonly Animator _animator;
        private readonly Collider _mainCollider;
        private readonly MonoRagdollFragment[] _fragments;
        private readonly Rigidbody _rigidBody;

        public PlayerDeathInGround(Animator animator, Collider mainCollider, MonoRagdollFragment[] fragments, EntityStateMachine stateMachine, Entity entity) : base(stateMachine, entity)
        {
            _animator = animator;
            _mainCollider = mainCollider;
            _fragments = fragments;
            _rigidBody = (this.Entity as EntityPlayer)?.Rb;
        }

        public override void Enter()
        {
            base.Enter();

            _animator.enabled = false;
            _mainCollider.enabled = false;
            _rigidBody.isKinematic = true;

            for (var i = 0; i < _fragments.Length; i++)
            {
                _fragments[i].Activate(Vector3.zero);
            }
            
            Object.Destroy(Entity.gameObject, 2f);
        }
    }
}