using System;
using Game.Scripts.DamageEffector;
using Game.Scripts.Player.EntityGame;
using Reflex.Attributes;
using UnityEngine;

namespace Game.Scripts.NPC.TargetAim
{
    public class TargetAim : Entity, IDamagable
    {
        public override bool HaveLife => true;
        public Vector3 Position => transform.position;
        public event Action<IDamagable, float> OnDamage;
        public EntityType CurrentType => EntityType.Pirate;

        private DamageEffectSpawner _effectSpawner;
        
        [Inject]
        private void Construct(DamageEffectSpawner effectSpawner)
        {
            _effectSpawner = effectSpawner;
        }
        
        public void TakeDamage(float damage, Vector3 pointDamage = default, bool critical = false)
        {
            if (_effectSpawner != null)
            {
                _effectSpawner.Spawn(transform.position, damage, critical, EntityType.Pirate);
            }
            Debug.Log($"Damage Target Aim: {damage}:{critical}");
        }
    }
}
