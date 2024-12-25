using Game.Scripts.NPC;
using Game.Scripts.Player.EntityGame;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Player.WeaponController.Weapons
{
    public class WeaponFirearmsShotGun : WeaponBehaviour
    {
        [SerializeField, FoldoutGroup("Settings")] private int _countBullet;
        [SerializeField, FoldoutGroup("Settings")] private float _angleShot;
        
        public override void Shot(EntityType target, Entity targetEntity)
        {
            base.Shot(target, targetEntity);
            var critical = Random.Range(0f, 100f) <= ChangeCritical();

            if (Vector3.Distance(Components.PointSpawnBullet.position, targetEntity.transform.position) < _distanceAutoAttack)
            {
                if (targetEntity is IDamagable damage)
                {
                    damage.TakeDamage(critical ? Damage() * 2f : Damage(), targetEntity.transform.position + Vector3.up ,critical: critical);
                    return;
                }
            }
            var angle = _angleShot * (_countBullet / 2f);
            for (var i = 0; i < _countBullet; i++)
            {
                var bullet = GetBullet();
                bullet.transform.position = Components.PointSpawnBullet.position;
                bullet.transform.SetParent(null);
                bullet.StartMove(DirectionMove(Components.PointSpawnBullet.forward, angle), target, critical ? Damage() * 2f : Damage(), critical);
                bullet.gameObject.SetActive(true);
                angle -= _angleShot;
            }
        }

        public override float Damage()
        {
            return base.Damage() / _countBullet;
        }

        private Vector3 DirectionMove(Vector3 forward, float angle)
        {
            forward.y += angle ;
            return forward;
        }
    }
}