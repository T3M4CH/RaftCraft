namespace Game.Scripts.Player.WeaponController.WeaponInventory
{
    public interface IInventoryWeapon
    {
        public void AddWeapon(WeaponId id);
        public WeaponItem GetCurrentSelected();
    }
}