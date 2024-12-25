using System.Collections.Generic;
using Game.Scripts.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.ShopController
{
    [CreateAssetMenu(fileName = "ShopFish", menuName = "Shop/ShopFish")]
    public class ShopData : ScriptableObject, IWindowObject
    {
        public string Patch => $"Shop/{name}";
        public object InstanceObject => this;
        public void CreateAsset()
        {
            
        }
        
        [field: SerializeField] public List<PriceItem> Price { get; private set; }

        [Button]
        private void AddItem()
        {
            var item = new PriceItem();
            item.Init();
            Price.Add(item);
        }
    }
}
