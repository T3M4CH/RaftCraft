using System;
using Cysharp.Threading.Tasks;
using Game.Scripts.Core;
using Game.Scripts.Core.Debug;
using Newtonsoft.Json;
using UnityEngine;

namespace Game.Scripts.Saves
{
    public class GameSave : MonoBehaviour
    {
        private SavePreset _current;


        private void Awake()
        {
            _current = new EasySavePreset();
            _current.Load();
        }
        
        public bool HighPriority => false;

        public bool HaveLoad()
        {
            return _current != null && _current.HaveLoad();
        }

        public bool HaveKey(string key)
        {
            return _current.HaveKey(key);
        }

        public T GetData<T>(string key, T defaultValue)
        {
            return _current.GetData(key, defaultValue);
        }

        public void SetData<T>(string key, T value)
        {
            _current.SetKey(key, value);
        }

        private bool HaveLoadSystem()
        {
            return true;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (HaveLoadSystem() == false)
            {
                return;
            }

            if (GTapCoreSettings.Instance.SaveCondition.HasFlag(HaveSaveCondition.HaveFocus) == false)
            {
                return;
            }

            if (hasFocus == false)
            {
                SaveData();
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (HaveLoadSystem() == false)
            {
                return;
            }

            if (GTapCoreSettings.Instance.SaveCondition.HasFlag(HaveSaveCondition.HavePause) == false)
            {
                return;
            }

            if (pauseStatus)
            {
                SaveData();
            }
        }

        private void OnApplicationQuit()
        {
            if (HaveLoadSystem() == false)
            {
                return;
            }

            if (GTapCoreSettings.Instance.SaveCondition.HasFlag(HaveSaveCondition.HaveQuit) == false)
            {
                return;
            }

            SaveData();
        }

        private void OnDisable()
        {
            if (HaveLoadSystem() == false)
            {
                return;
            }

            if (GTapCoreSettings.Instance.SaveCondition.HasFlag(HaveSaveCondition.HaveDisable) == false)
            {
                return;
            }

            SaveData();
        }

        private void OnDestroy()
        {
            if (HaveLoadSystem() == false)
            {
                return;
            }

            if (GTapCoreSettings.Instance.SaveCondition.HasFlag(HaveSaveCondition.HaveDestroy) == false)
            {
                return;
            }

            SaveData();
        }

        private void SaveData()
        {
            if (_current == null || _current.HaveLoad() == false)
            {
                return;
            }

            _current.Save();
        }
    }
}