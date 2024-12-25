using Sirenix.OdinInspector;
using UnityEngine.Audio;
using UnityEngine;
using System;
using GTapSoundManager.SoundManager;

namespace Game.Scripts.Sound
{
    [Serializable]
    public struct SerializableSoundManagerSettings
    {
        [field: SerializeField, AssetsOnly] public SoundAsset FightMusic { get; private set; } 
        [field: SerializeField, AssetsOnly] public SoundAsset PeacefulMusic { get; private set; } 
        [field: SerializeField, AssetsOnly] public AudioMixerGroup MusicMixer { get; private set; }
        [field: SerializeField, AssetsOnly] public AudioMixerGroup SoundsMixer { get; private set; }
    }
}