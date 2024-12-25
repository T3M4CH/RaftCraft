using Game.Scripts.Player.EntityGame;
using Game.Scripts.Player.StateMachine;

namespace Game.Scripts.Player.WeaponController.WeaponState
{
    public class IdleWeaponState : EntityState
    {
        private WeaponController _weaponController;
        
        public IdleWeaponState(EntityStateMachine stateMachine, Entity entity, WeaponController weaponController) : base(stateMachine, entity)
        {
            _weaponController = weaponController;
        }

        public override void Enter()
        {
            base.Enter();
            _weaponController.Rigging.SetWeigh(0f);
            _weaponController.Rigging.SetWeightSpine(0f);
        }
    }
}
