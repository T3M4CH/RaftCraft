using System.Collections.Generic;
using Game.Scripts.Core;
using Game.Scripts.NPC.Fish;
using UnityEngine;

namespace Game.Scripts.BattleMode
{
    [CreateAssetMenu(fileName = "EnemyProperty", menuName = "Enemy/EnemyProperty")]
    public class EnemyProperty : ScriptableObject, IWindowObject
    {
        [field: SerializeField] public int Health { get; private set; }
        [field: SerializeField] public int Damage { get; private set; }
        [field: SerializeField] public float SpeedMove { get; private set; }
        [field: SerializeField] public float CooldownUpdatePath { get; private set; }
        [field: SerializeField] public float SpeedRotate { get; private set; }
        [field: SerializeField] public float ReachedDistance { get; private set; }
        [field: SerializeField] public Color BodyColor { get; private set; }
        [field: SerializeField] public Vector2 ScaleMinMax { get; private set; }
        [field: SerializeField] public List<DropItems> Drop { get; private set; }

        public string Patch => $"EnemyProperty/{name}";
        public object InstanceObject => this;

        public void CreateAsset()
        {
        }
    }
}