using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace Game.Scripts.Quest.Structs
{
    [Serializable]
    public struct SerializableQuestPerDay
    {
        [field: SerializeField] public int DayId { get; private set; }
        [field: SerializeField] public MonoTutorialBase QuestPrefab { get; private set; }
        [field: SerializeField] public Object[] ExtraParams { get; set; }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public bool Equals(SerializableQuestPerDay other)
        {
            return DayId == other.DayId && Equals(QuestPrefab, other.QuestPrefab);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DayId, QuestPrefab);
        }
    }
}