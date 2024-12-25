using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.DamageEffector.Data
{
    public enum EffectType
    {
        DeadFish,
        DamageHumanBullet,
        DeadPlayerInWater,
        DeadPlayerInGround
    }
    
    [CreateAssetMenu(menuName = "GameData/EffectData", fileName = "EffectData")]
    public class EffectData : ScriptableObject, IWindowObject
    {
        [System.Serializable]
        public struct Data
        {
            [field: SerializeField] public EffectType Type { get; private set; }
            [field: SerializeField] public ParticleController Prefab { get; private set; }
        }


        public string Patch => "GameData/EffectData";
        public object InstanceObject => this;
        public void CreateAsset()
        {
            
        }
        

        [SerializeField, Header("Список игровых эффектов")] private List<Data> _effectData = new List<Data>();

        public bool TryGetEffect(EffectType type, out ParticleController prefab)
        {
            prefab = GetEffectFromType(type);
            if (prefab != null)
            {
                return true;
            }

            return false;
        }
        
        private ParticleController GetEffectFromType(EffectType type)
        {
            return (from effect in _effectData where effect.Type == type select effect.Prefab).FirstOrDefault();
        }
    }
}
