using UnityEngine;

namespace Game.Scripts.Player.WeaponController
{
    [System.Serializable]
    public class WeaponEntityData 
    {
        [field: SerializeField] public WeaponType Type { get; private set; }
        [field: SerializeField] public GameObject Link { get; private set; }
        [field: SerializeField] public WeaponBehaviour Behaviour { get; private set; }
    }
}
