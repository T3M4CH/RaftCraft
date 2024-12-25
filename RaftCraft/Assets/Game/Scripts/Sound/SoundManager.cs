using Game.Scripts.Sound.Interfaces;
using Game.Scripts.GameSettings;
using Game.Scripts.Sound;
using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GTapSoundManager.SoundManager;
using Object = UnityEngine.Object;

public class SoundManager : IMusicService, IStartableElement, IDisposable
{
    //TODO: Вынести в ProjectInstaller

    public SoundManager(IUpdateGameSettings updateGameSettings, SerializableSoundManagerSettings soundManagerSettings)
    {
        _fightMusic = soundManagerSettings.FightMusic;
        _peacefulMusic = soundManagerSettings.PeacefulMusic;

        _updateGameSettings = updateGameSettings;
        _musicMixer = soundManagerSettings.MusicMixer;
        _soundMixer = soundManagerSettings.SoundsMixer;

        _updateGameSettings.OnChangeSound += ValidateMixers;
        _updateGameSettings.OnChangeMusic += ValidateMixers;
        _updateGameSettings.OnChangeHaptic += ValidateHaptic;
        
        PlayMusic(EMusicType.Peaceful, 2);
    }

    private float _lastAttacked;
    private float _lastSpawnedTime;
    private SoundAsset _currentMusic;
    private SoundAsset[] _soundAssets;
    private CancellationTokenSource _cancellationTokenSource;

    private readonly float _spawnDelay;
    private readonly float _attackDelay;
    private readonly SoundAsset _fightMusic;
    private readonly SoundAsset _peacefulMusic;
    private readonly AudioMixerGroup _musicMixer;
    private readonly AudioMixerGroup _soundMixer;
    private readonly IUpdateGameSettings _updateGameSettings;

    public void PlayMusic(EMusicType musicType, float? delay = null)
    {
        try
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();

                _cancellationTokenSource = null;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        
        if (delay.HasValue)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;
            
            PlayMusicAsync(musicType, delay.Value, token).Forget();
            
            return;
        }

        switch (musicType)
        {
            case EMusicType.Fight:
                _peacefulMusic.StopClip();
                _fightMusic.Play();
                _currentMusic = _fightMusic;
                break;
            case EMusicType.Peaceful:
                _fightMusic.StopClip();
                _peacefulMusic.Play();
                _currentMusic = _peacefulMusic;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(musicType), musicType, null);
        }
        
        _currentMusic.SetVolume(1);
    }

    public void StopMusic()
    {
        _fightMusic.StopClip();
        _peacefulMusic.StopClip();
    }

    public void Play(AudioClip clip, AudioSource source)
    {
        source.PlayOneShot(clip);
    }

    public void PlayLoop(AudioClip clip, AudioSource source)
    {
        source.clip = clip;
        source.loop = true;
        source.Play();
    }

    private async UniTaskVoid PlayMusicAsync(EMusicType musicType, float delay, CancellationToken token)
    {
        var currentTime = 0f;

        var nextMusic = _currentMusic;
        
        switch (musicType)
        {
            case EMusicType.Fight:
                nextMusic = _fightMusic;
                break;
            case EMusicType.Peaceful:
                nextMusic = _peacefulMusic;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(musicType), musicType, null);
        }

        if (nextMusic == _currentMusic)
        {
            _currentMusic = null;
        }
        
        while (currentTime < delay)
        {
            await UniTask.Yield(token).SuppressCancellationThrow();

            if (token.IsCancellationRequested)
            {
                return;
            }

            if (_updateGameSettings.Music)
            {
                var ratio = currentTime / delay;
                
                nextMusic.SetVolume(ratio);
                
                _currentMusic?.SetVolume(1 - ratio);
            }
            
            currentTime += Time.deltaTime;
        }
        
        PlayMusic(musicType);
    }
    
    private void ValidateMixers(bool _ = default)
    {
        _musicMixer.audioMixer.SetFloat("VolumeMusic", _updateGameSettings.Music ? 0 : -80);
        _soundMixer.audioMixer.SetFloat("VolumeSound", _updateGameSettings.Sound ? 0 : -80);
    }

    private void ValidateHaptic(bool _ = default)
    {
        for (var i = 0; i < _soundAssets.Length; i++)
        {
            _soundAssets[i].ChangeHapticState(_updateGameSettings.Haptic);
        }
    }

    public void Dispose()
    {
        try
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel(false);
                _cancellationTokenSource.Dispose();

                _cancellationTokenSource = null;
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        
        _updateGameSettings.OnChangeSound -= ValidateMixers;
        _updateGameSettings.OnChangeMusic -= ValidateMixers;
        _updateGameSettings.OnChangeHaptic -= ValidateHaptic;
    }

    public void Execute()
    {
        _soundAssets = Resources.LoadAll<SoundAsset>("SoundAssets");
        var parent = new GameObject("SoundAssets").transform;
        foreach (var soundAsset in _soundAssets)
        {
            soundAsset.Initialize(parent);
        }
        
        ValidateMixers();
        ValidateHaptic();
    }

    public int Priority => 10000;
}