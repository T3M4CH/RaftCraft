using System;
using System.Collections;
using Game.Scripts.BattleMode;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.TimeController
{
    public class RenderTimeController : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("Settings")] private Renderer _renderer;
        [SerializeField, FoldoutGroup("Settings")] private Material _dayMaterial;
        [SerializeField, FoldoutGroup("Settings")] private Material _nightMaterial;
        [SerializeField, FoldoutGroup("Settings")] private float _speedLerp;
        
        private Material _temp;
        private IBattleService _battleService;

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
                        _temp = new Material(_nightMaterial);
                        StopAllCoroutines();
                        StartCoroutine(LerpMaterial(_dayMaterial));
                        break;
                    case TimeState.Night:
                        _temp = new Material(_dayMaterial);
                        StopAllCoroutines();
                        StartCoroutine(LerpMaterial(_nightMaterial));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        [Inject]
        private void Construct(IBattleService battleService)
        {
            _battleService = battleService;
        }

        private IEnumerator LerpMaterial(Material target)
        {
            var progress = 0f;
            while (progress < 1f)
            {
                _temp.Lerp(_temp, target, progress);
                progress += Time.smoothDeltaTime * _speedLerp;
                _renderer.sharedMaterial = _temp;
                yield return null;
            }

            _renderer.sharedMaterial = target;
        }
        
        private void Start()
        {
            if (_battleService != null)
            {
                _battleService.OnChangeState += BattleServiceOnOnChangeState;
            }
        }

        private void BattleServiceOnOnChangeState(BattleState state)
        {
            switch (state)
            {
                case BattleState.СutScene:
                    State = TimeState.Night;
                    break;
                case BattleState.Fight:
                    State = TimeState.Night;
                    break;
                case BattleState.Result:
                    break;
                case BattleState.Idle:
                    State = TimeState.Day;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void OnDestroy()
        {
            if (_battleService != null)
            {
                _battleService.OnChangeState -= BattleServiceOnOnChangeState;
            }
        }
    }
}
