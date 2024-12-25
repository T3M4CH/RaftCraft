using System;
using Game.Scripts.BattleMode;
using Reflex.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.NPC.Vendor
{
    public class VendorAnimationController : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("Settings")] private Animator _animator;

        private readonly int _hasPanic = Animator.StringToHash("Panic");
        private readonly int _hasSpeed = Animator.StringToHash("Speed");

        private IBattleService _battleService;
        
        [Inject]
        private void Construct(IBattleService battleService)
        {
            _battleService = battleService;
        }

        private void Start()
        {
            _battleService.OnChangeState += BattleServiceOnOnChangeState;
        }

        private void BattleServiceOnOnChangeState(BattleState state)
        {
            switch (state)
            {
                case BattleState.СutScene:
                    break;
                case BattleState.Fight:
                    _animator.SetBool(_hasPanic, true);
                    _animator.SetFloat(_hasSpeed, Random.Range(0.8f, 1.2f));
                    break;
                case BattleState.Result:
                    break;
                case BattleState.Idle:
                    _animator.SetBool(_hasPanic, false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void OnDestroy()
        {
            _battleService.OnChangeState -= BattleServiceOnOnChangeState;
        }
    }
}
