using Game.Scripts.Core;
using Game.Scripts.ResourceController.Enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.BattleMode
{
    [CreateAssetMenu(fileName = "EnemySets", menuName = "Enemy/EnemySets")]
    public class EnemySets : ScriptableObject, IWindowObject
    {
        [field: SerializeField] public SerializableEnemySetPerDay[] EnemySetPerDay { get; private set; }

        public string Patch => $"EnemySets/{name}";
        public object InstanceObject => this;

        [Button]
        private void Fix()
        {
            foreach (var set in EnemySetPerDay)
            {
                foreach (var item in set.DropItems)
                {
                    item.ResourceType = EResourceType.Wood;
                }
            }
        }
        public void CreateAsset()
        {
        }
    }
}