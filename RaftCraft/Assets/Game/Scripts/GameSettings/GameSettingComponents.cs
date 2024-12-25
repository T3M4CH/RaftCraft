using UnityEngine;

namespace Game.Scripts.GameSettings
{
    [System.Serializable]
    public struct GameSettingComponents 
    {
        [field: SerializeField] public Sprite IconSound { get; private set; }
        [field: SerializeField] public Sprite IconMusic { get; private set; }
        [field: SerializeField] public Sprite IconHaptic { get; private set; }
        [field: SerializeField] public Sprite IconSoundDisabled { get; private set; }
        [field: SerializeField] public Sprite IconMusicDisabled { get; private set; }
        [field: SerializeField] public Sprite IconHapticDisabled { get; private set; }

    }
}
