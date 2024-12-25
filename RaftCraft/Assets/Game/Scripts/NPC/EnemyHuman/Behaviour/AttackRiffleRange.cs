using Game.GameBalanceCore.scripts;
using Game.GameBalanceCore.Scripts.BalanceValue;
using UnityEngine;

namespace Game.Scripts.NPC
{
    public class AttackRiffleRange: IAttack
    {
        private readonly int _damage;

        public AttackRiffleRange(int damage, Transform pointBulletSpawn)
        {
            _damage = damage;
        }

        public void Attack(IDamagable damagable)
        {
            //TODO: спавн пули с указанием куда ее лететь и в указанной точке самоуничтожиться с выполнением метода ниже
            //TODO: Получать координаты в которых пуля нансела урон
            damagable.TakeDamage(GameBalance.Instance.GetBalanceValue(_damage, TypeValueBalance.PirateDamage), Vector3.zero);
        }
    }
}