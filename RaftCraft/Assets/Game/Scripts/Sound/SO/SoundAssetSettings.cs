using UnityEngine;
using UnityEngine.Audio;

namespace GTapSoundManager.SoundManager
{
    [System.Serializable]
    public class SoundAssetSettings
    {
        #region Private Serialize Field

        [SerializeField] private SoundAssetType _assetType;
        [SerializeField] private AudioMixerGroup _audioMixer;
        [SerializeField] private bool _isLoop = false;
        [SerializeField] private float _volume = 1f;
        [SerializeField] private bool _isRandomSound = false;
        [SerializeField] private bool _playOnEnabled = false;
        [SerializeField] private bool _waitPlay = false;
        [SerializeField, Range(0f, 1f)] private float _spatialBlend = 0f;
        [SerializeField] private float _minDistance;
        [SerializeField] private float _maxDistence;

        #endregion

        #region Public Property

        public SoundAssetType AssetType => _assetType;
        public AudioMixerGroup MixerGroup => _audioMixer;
        public bool IsLoop => _isLoop;
        public float Volume => _volume;
        public bool IsRandom => _isRandomSound;

        public bool PlayOnEnabled => _playOnEnabled;

        public bool WaitPlay => _waitPlay;

        public float SpatialBlend => _spatialBlend;

        public float MinDistance => _minDistance;

        public float MaxDistance => _maxDistence;

        #endregion
    }
}