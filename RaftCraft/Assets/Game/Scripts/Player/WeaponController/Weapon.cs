using Game.Scripts.Player.RigginController;

namespace Game.Scripts.Player.WeaponController
{
    public class Weapon
    {
        public WeaponEntityData WeaponEntityData { get; private set; }
        
        private HumanoidRiggingWeapon _rigging;
        
        private bool _isActive;

        private bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                if (WeaponEntityData == null)
                {
                    return;
                }

                if (WeaponEntityData.Link != null)
                {
                    WeaponEntityData.Link.SetActive(_isActive);
                }
                if (_rigging == null)
                {
                    return;
                }
                
                _rigging.SetCurrentWeaponModel(IsActive ? WeaponEntityData : null);
            }
        }
        
        public Weapon(WeaponEntityData data, HumanoidRiggingWeapon riggingWeapon)
        {
            WeaponEntityData = data;
            _rigging = riggingWeapon;
        }

        public void Use()
        {
            IsActive = true;
        }

        public void Hide()
        {
            WeaponEntityData.Behaviour.Hide();
            IsActive = false;
        }
    }
}
