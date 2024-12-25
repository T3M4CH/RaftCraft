using System;
using System.Collections.Generic;
using Game.Scripts.Core;
using Game.Scripts.DamageEffector.Data;
using Game.Scripts.DamageEffector.Interface;
using Game.Scripts.NPC.Interface;
using Game.Scripts.Player.EntityGame;
using Game.Scripts.Pool;
using Reflex.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Scripts.DamageEffector
{
    public class GameEffectSpawner : IStartable, ISpawnEffectService
    {
        private Dictionary<EffectType, PoolObjects<ParticleController>> _poolEffect;
        
        public GameEffectSpawner()
        {
            var effects = Resources.Load<EffectData>("Effects/EffectData");

            _poolEffect = new Dictionary<EffectType, PoolObjects<ParticleController>>();
            foreach (EffectType type in (EffectType[]) Enum.GetValues(typeof(EffectType)))
            {
                var content = new GameObject($"Content_{type}").transform;
                Object.DontDestroyOnLoad(content.gameObject);
                if(effects.TryGetEffect(type, out var prefab))
                {
                    _poolEffect.TryAdd(type, new PoolObjects<ParticleController>(prefab, content, 5));
                }
            }
        }

        public ParticleController SpawnEffect(EffectType type, Vector3? position = null, Quaternion? rotation = null)
        {
            if (_poolEffect.ContainsKey(type) == false)
            {
                return null;
            }

            var effect = _poolEffect[type].GetFree();
            if (position != null) effect.transform.position = position.Value;
            if (rotation != null) effect.transform.rotation = rotation.Value;
            effect.gameObject.SetActive(true);
            return effect;
        }
        
        public void Start()
        {
            
        }
    }
}
