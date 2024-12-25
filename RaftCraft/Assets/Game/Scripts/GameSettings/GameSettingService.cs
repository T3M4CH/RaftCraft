using System;
using Game.Scripts.Saves;
using Game.Scripts.UI.WindowManager;
using Game.Scripts.UI.WindowManager.Windows;
using Lofelt.NiceVibrations;
using Reflex.Core;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Scripts.GameSettings
{
    public class GameSettingService : IUpdateGameSettings, IDisposable
    {
        //TODO: Вынести в ProjectInstaller
        
        public event Action<bool> OnChangeMusic;
        public event Action<bool> OnChangeHaptic;
        public event Action<bool> OnChangeSound;

        private WindowManager _windowManager;
        private CellGameSettings _prefabCell;

        private bool _sounds;
        private bool _haptic;
        private bool _music;
        private GameSave _save;
        private CellGameSettings _cellSound;
        private CellGameSettings _cellMusic;
        private CellGameSettings _cellHaptic;
        private GameSettingComponents _components;

        public GameSettingService(WindowManager windowManager, CellGameSettings cellSetting, GameSave save, GameSettingComponents components)
        {
            _save = save;
            _windowManager = windowManager;
            _prefabCell = cellSetting;
            _components = components;
            
            InitUI();
        }

        private void InitUI()
        {
            Sound = _save.GetData(SaveConstants.SoundSettings, true);
            Music = _save.GetData(SaveConstants.MusicSettings, true);
            Haptic = _save.GetData(SaveConstants.HapticSettings, true);
            _cellSound = CreateCell(_components.IconSound, _components.IconSoundDisabled, "Sound", Sound);
            _cellMusic = CreateCell(_components.IconMusic, _components.IconMusicDisabled, "Music", Music);
            _cellHaptic = CreateCell(_components.IconHaptic, _components.IconHapticDisabled, "Haptic", Haptic);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_windowManager.GetWindow<WindowSettings>().ContentSettings);

            _cellMusic.Toggle.StateChanged += ChangeMusic;
            _cellHaptic.Toggle.StateChanged += ChangeHaptic;
            _cellSound.Toggle.StateChanged += ChangeSound;
        }

        private void ChangeMusic(bool state)
        {
            Music = state;
        }

        private void ChangeHaptic(bool state)
        {
            Haptic = state;
        }
        
        private void ChangeSound(bool state)
        {
            Sound = state;
        }

        private CellGameSettings CreateCell(Sprite icon, Sprite disabled, string nameCell, bool defaultValue)
        {
            var cell = Object.Instantiate(_prefabCell, _windowManager.GetWindow<WindowSettings>().ContentSettings);
            cell.Init(defaultValue, icon, disabled, nameCell);
            return cell;
        }

        public void Dispose()
        {
            if (_cellMusic == null || _cellHaptic == null)
            {
                return;
            }

            _cellMusic.Toggle.StateChanged -= ChangeMusic;
            _cellHaptic.Toggle.StateChanged -= ChangeHaptic;
            _cellSound.Toggle.StateChanged -= ChangeSound;
        }

        public bool Music
        {
            get => _music;
            set
            {
                _music = value;
                OnChangeMusic?.Invoke(value);
                _save.SetData(SaveConstants.MusicSettings, _music);
            }
        }

        public bool Haptic
        {
            get => _haptic;

            set
            {
                _haptic = value;
                OnChangeHaptic?.Invoke(value);
                HapticController.hapticsEnabled = _haptic;
                _save.SetData(SaveConstants.HapticSettings, _haptic);
            }
        }

        public bool Sound
        {
            get => _sounds;

            set
            {
                _sounds = value;
                OnChangeSound?.Invoke(value);
                _save.SetData(SaveConstants.SoundSettings, _sounds);
            }
        }
    }
}