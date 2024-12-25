using System.Collections.Generic;
using Lofelt.NiceVibrations;
using MoreMountains.Feedbacks;
using MoreMountains.FeedbacksForThirdParty;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GTapSoundManager.SoundManager
{
    public class SoundAsset : ScriptableObject
    {
        [SerializeField] private List<AudioClip> _audioClips = new();
        [SerializeField] private SoundAssetSettings _settings;
        [SerializeField] private HapticPatterns.PresetType hapticPattern;

        private float _maxVolume;
        private bool _hapticEnabled;
        private bool _isInitialized;
        private AudioSource _source;
        private Transform _localPoint;
        public SoundAssetSettings Settings => _settings;

        public AudioSource Source => _source;

        public void Initialize(Transform parent)
        {
            if (_source != null)
            {
                Destroy(_source.gameObject);
            }

            if (_audioClips.Count == 0)
            {
                Debug.LogError($"{name}: There are no sounds in the asset!!");
                return;
            }

            _source = new GameObject($"{_audioClips[0].name}").AddComponent<AudioSource>();
            _source.transform.SetParent(parent);
            _source.outputAudioMixerGroup = _settings.MixerGroup;
            _source.loop = _settings.IsLoop;
            _source.volume = _settings.Volume;
            _source.playOnAwake = _settings.PlayOnEnabled;
            _source.spatialBlend = _settings.SpatialBlend;
            _source.minDistance = _settings.MinDistance;
            _source.maxDistance = _settings.MaxDistance;
            _maxVolume = _source.volume;
        }

        public void InitClip(AudioClip clip)
        {
            _audioClips.Add(clip);
        }

        public void SetTargetPoint(Transform point)
        {
            _localPoint = point;
            _source.transform.SetParent(_localPoint);
            _source.transform.localPosition = Vector3.zero;
        }

        public void ChangeHapticState(bool value)
        {
            _hapticEnabled = value;
        }

        public void SetVolume(float value)
        {
            if(!_source) return;
            _source.volume = Mathf.Lerp(0, _maxVolume, value);
        }

        public void Play(float? pitch = null)
        {
            if (_source == null)
            {
                return;
            }

            if (_settings.WaitPlay)
            {
                if (IsPlay())
                {
                    return;
                }
            }

            if (_hapticEnabled)
            {
                HapticPatterns.PlayPreset(hapticPattern);
            }

            switch (_settings.AssetType)
            {
                case SoundAssetType.Sound:
                    PlayOneShot(_settings.IsRandom ? _audioClips[Random.Range(0, _audioClips.Count)] : _audioClips[0], pitch);
                    break;
                case SoundAssetType.Music:
                    PlayClip(_settings.IsRandom ? _audioClips[Random.Range(0, _audioClips.Count)] : _audioClips[0], pitch);
                    break;
            }
        }

        public bool IsPlay()
        {
            return _source != null && _source.isPlaying;
        }

        public void StopClip()
        {
            if (_source != null)
            {
                _source.Stop();
            }
        }

        public SoundAsset GetSoloSource()
        {
            var asset = Instantiate(this);
            return asset;
        }

        private void PlayClip(AudioClip clip, float? pitch = null)
        {
            if (pitch.HasValue)
            {
                _source.pitch = pitch.Value;
            }

            _source.clip = clip;
            _source.Play();
        }

        private void PlayOneShot(AudioClip clip, float? pitch = null)
        {
            if (_settings.WaitPlay)
            {
                PlayClip(clip, pitch);
            }
            else
            {
                if (pitch.HasValue)
                {
                    _source.pitch = pitch.Value;
                }

                _source.clip = clip;
                _source.PlayOneShot(clip, _settings.Volume);
            }
        }
    }
}