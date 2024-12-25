using Game.Scripts.ResourceController.Enums;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Game.Scripts.ShopController
{
    [System.Serializable]
    public class PriceItem
    {
        [field: SerializeField, ReadOnly] public string Id { get; private set; }
        [field: SerializeField]public EResourceType Input { get; private set; }
        [field: SerializeField]public EResourceType Output { get; private set; }
        [field: SerializeField]public int CountInput { get; private set; }
        [field: SerializeField]public int CountOutput { get; private set; }
        [field: SerializeField] public float DelayCell { get; private set; }

        public void Init()
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(Id))
            {
                Id = GUID.Generate().ToString();
            }
#endif
        }
    }
}