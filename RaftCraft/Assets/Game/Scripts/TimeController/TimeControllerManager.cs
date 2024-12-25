using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scripts.TimeController
{
    public class TimeControllerManager : MonoBehaviour, ITimeController
    {
        [SerializeField]
        private TimeControllerData _data;

        [SerializeField, FoldoutGroup("Settings")] private Transform _dayRotation;
        [SerializeField, FoldoutGroup("Settings")] private Transform _nightRotation;

        [SerializeField, ReadOnly]
        private Vector3 _currentRotation;
        
        private TimeState _state;

        private TimeState State
        {
            get => _state;
            set
            {
                if (_state == value)
                {
                    return;
                }
                _state = value;
                switch (_state)
                {
                    case TimeState.Day:
                        _tempMaterial = new Material(_data.NightMaterial);
                        StopAllCoroutines();
                        StartCoroutine(WaitLerp(_data.DayMaterial, _dayRotation.forward));
                        break;
                    case TimeState.Night:
                        _tempMaterial = new Material(_data.DayMaterial);
                        StopAllCoroutines();
                        StartCoroutine(WaitLerp(_data.NightMaterial, _nightRotation.forward));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private Material _tempMaterial;

        [Button]
        public void SetDay()
        {
            State = TimeState.Day;
        }

        [Button]
        public void SetNight()
        {
            State = TimeState.Night;
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
        }

        private void SceneManagerOnsceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _currentRotation = _dayRotation.forward;
            Shader.SetGlobalVector("GlobalMoonDirection", _currentRotation);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                State = TimeState.Day;
            }
            
            if (Input.GetKeyDown(KeyCode.K))
            {
                State = TimeState.Night;
            }
           // Shader.SetGlobalVector("GlobalMoonDirection", State == TimeState.Day ? _dayRotation.forward : _nightRotation.forward);
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= SceneManagerOnsceneLoaded;
        }

        private IEnumerator WaitLerp(Material target, Vector3 targetMoon)
        {
            var progress = 0f;
            while (progress < 1f)
            {
                _tempMaterial.Lerp(_tempMaterial, target, progress);
                _currentRotation = Vector3.Lerp(_currentRotation, targetMoon, progress);
                Shader.SetGlobalVector("GlobalMoonDirection", _currentRotation);
                RenderSettings.skybox = _tempMaterial;
                progress += Time.smoothDeltaTime * _data.SpeedLerp;
                yield return null;
            }
            
            RenderSettings.skybox = target;
        }
    }
}
