using System;
using Game.Scripts.Player.EntityGame;
using UnityEngine;

namespace Game.Scripts.NPC
{
    public interface IDamagable
    {
        public Vector3 Position { get; }
        public event Action<IDamagable, float> OnDamage;
        public EntityType CurrentType { get; }
        public void TakeDamage(float damage, Vector3 pointDamage = default, bool critical = false);
    }
}