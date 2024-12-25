using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.Player.WeaponController.BulletSystem
{
    [CreateAssetMenu(menuName = "Bullet/Settings", fileName = "BulletSettings")]
    public class BulletSettings : ScriptableObject, IWindowObject
    {
        public string Patch => $"Bullets/{name}";
        public object InstanceObject => this;
        public void CreateAsset()
        {
            
        }
        
        [field: SerializeField] public float SpeedMove { get; private set; }
        [field: SerializeField] public float TimeLife { get; private set; }
    }
}
