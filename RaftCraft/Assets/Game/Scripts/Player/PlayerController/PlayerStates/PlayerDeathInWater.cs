using Game.Scripts.DamageEffector.Data;
using Game.Scripts.DamageEffector.Interface;
using Game.Scripts.Player.EntityGame;
using Game.Scripts.Player.StateMachine;
using GTap.Analytics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Scripts.Player.PlayerController.PlayerStates
{
    public class PlayerDeathInWater : EntityState
    {
        private readonly ISpawnEffectService _particleEffect;
        private readonly Animator _animator;
        private readonly Collider _mainCollider;
        private readonly MonoRagdollFragment[] _fragments;
        private readonly Rigidbody _rigidBody;

        public PlayerDeathInWater(Animator animator, Collider mainCollider, MonoRagdollFragment[] fragments, ISpawnEffectService effectService, EntityStateMachine stateMachine, Entity entity) : base(stateMachine, entity)
        {
            _particleEffect = effectService;
            
            _animator = animator;
            _mainCollider = mainCollider;
            _fragments = fragments;
            _rigidBody = (this.Entity as EntityPlayer)?.Rb;
        }

        public override void Enter()
        {
            base.Enter();

            GtapAnalytics.DeathInWater += 1;
            Entity.gameObject.layer = 0;
            _animator.enabled = false;
            _mainCollider.enabled = false;
            _rigidBody.isKinematic = true;

            for (var i = 0; i < _fragments.Length; i++)
            {
                _fragments[i].SetGravity(-16.5f);
                _fragments[i].Activate(Vector3.zero);
            }
            
            Object.Destroy(Entity.gameObject, 2f);
            _particleEffect.SpawnEffect(EffectType.DeadPlayerInWater, Entity.transform.position);
            //_particleEffect.transform.SetParent(null);
            //_particleEffect.SetActive(true);
            //Object.Destroy(Entity.gameObject);
        }
    }
}