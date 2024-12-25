using System;
using Game.Scripts.CameraSystem.Enums;
using UnityEngine;

namespace Game.Scripts.Player.Spawners
{
    [Serializable]
    public struct SerializablePlayerSpawnerSettings
    {
        [field:SerializeField] public Vector3 SpawnPosition { get; private set; }
        [field: SerializeField] public ECameraType CameraType { get; private set; }
    }
}