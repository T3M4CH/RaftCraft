using Game.GameBalanceCore.scripts;
using Game.GameBalanceCore.Scripts.BalanceValue;
using Game.Scripts.Player.EntityGame;

namespace Game.Scripts.NPC
{
    public class AttackMeleeBehavior : IAttack
    {
        private readonly int _damage;
        private readonly Entity _current;

        public AttackMeleeBehavior(int damage, Entity current)
        {
            _damage = damage;
            _current = current;
        }

        //TODO Тут необходимо прокидывать оружие или руку которой бьет пират
        public void Attack(IDamagable damagable)
        {
            if (damagable == null || _current == null)
            {
                return;
            }
            damagable.TakeDamage(GameBalance.Instance.GetBalanceValue(_damage, TypeValueBalance.PirateDamage),
                _current.transform.position);
        }
    }
}