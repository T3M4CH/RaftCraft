using Game.Scripts.NPC;
using Game.Scripts.Player.EntityGame;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Player.WeaponController.Weapons
{
    public class WeaponFirearmsOneShot : WeaponBehaviour
    {
        public override void Shot(EntityType target, Entity targetEntity)
        {
            base.Shot(target, targetEntity);
            var critical = Random.Range(0f, 100f) <= ChangeCritical();

            if (Vector3.Distance(Components.PointSpawnBullet.position, targetEntity.transform.position) < _distanceAutoAttack)
            {
                if (targetEntity is IDamagable damage)
                {
                    damage.TakeDamage(critical ? Damage() * 2f : Damage(), critical: critical);
                    return;
                }
            }
            var bullet = GetBullet();
            bullet.transform.position = Components.PointSpawnBullet.position;
            bullet.transform.SetParent(null);
            bullet.StartMove(SpreadForward(Components.PointSpawnBullet.forward), target, critical ? Damage() * 2f : Damage(), critical);
            bullet.gameObject.SetActive(true);
        }
    }
}