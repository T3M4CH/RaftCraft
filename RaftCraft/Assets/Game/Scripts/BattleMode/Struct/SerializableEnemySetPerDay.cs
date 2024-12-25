using System;
using Game.Scripts.NPC;
using Game.Scripts.NPC.Fish;
using UnityEngine;

namespace Game.Scripts.BattleMode
{
    [Serializable]
    public struct SerializableEnemySetPerDay
    {
        [field: SerializeField] public int DayId { get; private set; }
        [field: SerializeField] public EnemyRaft[] EnemyRaft { get; private set; }
        [field: SerializeField] public SerializableEnemyList[] Queue { get; private set; }
        [field: SerializeField] public DropItems[] DropItems { get; private set; }
    }
    
    [Serializable]
    public struct SerializableEnemyList
    {
        [field: SerializeField] public HumanBase Human { get; private set; }
        [field: SerializeField] public int Amount { get; private set; }
    }
}