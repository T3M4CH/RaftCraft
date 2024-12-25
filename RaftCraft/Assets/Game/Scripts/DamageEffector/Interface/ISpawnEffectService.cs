using Game.Scripts.Core;
using Game.Scripts.DamageEffector.Data;
using UnityEngine;

namespace Game.Scripts.DamageEffector.Interface
{
    public interface ISpawnEffectService
    {
        public ParticleController SpawnEffect(EffectType type, Vector3? position = null, Quaternion? rotation = null);
    }
}
