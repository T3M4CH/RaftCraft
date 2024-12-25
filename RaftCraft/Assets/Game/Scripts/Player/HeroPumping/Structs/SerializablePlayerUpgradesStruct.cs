using System;
using UnityEngine;

namespace Game.Scripts.Player.HeroPumping.Structs
{
    [Serializable]
    public struct SerializablePlayerUpgradesStruct
    {
        [field: SerializeField] public int Capacity { get; private set; }
        [field: SerializeField] public int FishLevels { get; private set; }
        [field: SerializeField] public float MaxDepth { get; private set; }
        [field: SerializeField] public float MaxHealth { get; private set; }
        [field: SerializeField] public float RaftSpeed { get; private set; }
        [field: SerializeField] public float WaterSpeed { get; private set; }
        [field: SerializeField] public float Oxygen { get; private set; }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public bool Equals(SerializablePlayerUpgradesStruct other)
        {
            return Capacity == other.Capacity && FishLevels == other.FishLevels && MaxDepth.Equals(other.MaxDepth) && MaxHealth.Equals(other.MaxHealth) && RaftSpeed.Equals(other.RaftSpeed) && WaterSpeed.Equals(other.WaterSpeed) && Oxygen.Equals(other.Oxygen);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Capacity, FishLevels, MaxDepth, MaxHealth, RaftSpeed, WaterSpeed, Oxygen);
        }
    }
}