using UnityEngine;
using System;

[Serializable]
public struct SerializablePlayerPriceStruct
{
    [field: SerializeField] public int AqualangPrice { get; private set; }
    [field: SerializeField] public int MaxHealth { get; private set; }
    [field: SerializeField] public int RaftSpeed { get; private set; }
}