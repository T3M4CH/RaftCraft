using System.Collections.Generic;
using Game.Scripts.Core;
using Game.Scripts.Core.DataParser;
using Game.Scripts.Player.WeaponController.BulletSystem;
using UnityEngine;

namespace Game.Scripts.Player.WeaponController.WeaponsData
{
    [CreateAssetMenu(menuName = "Weapons/DateWeapon", fileName = "WeapondData")]
    public class WeaponsDataSettings : ScriptableObjectParser<List<WeaponsDataSettings.WeaponLevelData>>, IWindowObject
    {
        [System.Serializable]
        public class WeaponLevelData
        {
            [field: SerializeField] public int Cost { get; set; }
            [field: SerializeField] public float Damage { get; set; }
            [field: SerializeField] public float DistanceAttack { get; set; }
            [field: SerializeField] public float TimeReload { get; set; }
            [field: SerializeField] public float Spread { get; set; }
            [field: SerializeField] public float ChangeCritical { get; set; }
        }
        
        public string Patch => $"Weapon/{name}";
        public object InstanceObject => this;
        public void CreateAsset()
        {
            
        }
        [field: SerializeField] public WeaponId CurrentId { get; private set; }
        [SerializeField] private List<WeaponLevelData> _levelUpgrade = new List<WeaponLevelData>();
        
        public float Damage(int level)
        {
            return GetDataLevel(level).Damage;
        }

        public float DistanceAttack(int level)
        {
            return GetDataLevel(level).DistanceAttack;
        }

        public float TimeReload(int level)
        {
            return GetDataLevel(level).TimeReload;
        }

        public float Spread(int level)
        {
            return GetDataLevel(level).Spread;
        }

        public int Cost(int level)
        {
            return GetDataLevel(level).Cost;
        }

        public float ChangeCritical(int level)
        {
            return GetDataLevel(level).ChangeCritical;
        }
        private WeaponLevelData GetDataLevel(int level)
        {
            return _levelUpgrade[Mathf.Clamp(level, 0, _levelUpgrade.Count - 1)];
        }
        
        [field: SerializeField] public Bullet PrefabBullet { get; private set; }
        public override void LoadParse(List<WeaponLevelData> result)
        {
            _levelUpgrade = result;
        }
    }
}
