using UnityEngine;

namespace Game.Scripts.DamageEffector.Data
{
    [System.Serializable]
    public struct EffectTextDamageData
    {
        [field: SerializeField] public DamageText DamageTextPirate { get; private set; }
        [field: SerializeField] public DamageText DamageTextPlayer { get; private set; }
    }
}