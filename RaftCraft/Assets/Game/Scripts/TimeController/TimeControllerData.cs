using UnityEngine;

namespace Game.Scripts.TimeController
{
    [System.Serializable]
    public struct TimeControllerData
    {
        [field: SerializeField] public Material DayMaterial { get; private set; }
        [field: SerializeField] public Material NightMaterial { get; private set; }
        [field: SerializeField] public float SpeedLerp { get; private set; }
    }
}