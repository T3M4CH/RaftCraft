using UnityEngine;
using System;
using Cinemachine;

namespace Game.Scripts.CameraSystem
{
    [Serializable]
    public struct SerializableCamerasSettings
    {
        [field: SerializeField] public CinemachineBrain MainCameraBrain { get; private set; }
        [field: SerializeField] public BaseVirtualCamera[] Cameras { get; private set; }
    }
}