using Game.Scripts.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.NPC
{
    public class MeleeHuman : HumanBase
    {
        #region Fields
        
        [SerializeField] private AnimatorHelperEvent _animatorHelper;
        
        private readonly int _runHash = Animator.StringToHash("Move");
        private readonly int _attackHash = Animator.StringToHash("Attack");
        private readonly int _idleBlend = Animator.StringToHash("IdleBlend");

        #endregion

        #region Init

        protected override void OnInit()
        {
            _moveBehavior = new Movement(transform, _rigidbody, _collider, _animator, _speedMoveModified, _property.SpeedRotate);
            _attackBehavior = new AttackMeleeBehavior(_property.Damage, this);
            
            _animatorHelper.OnAnimationPunchEvent += OnPunch;
            _animatorHelper.OnAnimationDeathEvent += OnDeath;
        }

        #endregion

        #region State

        protected override void EnterIdle()
        {
            _animator.CrossFade(_idleBlend, Random.Range(0f, 0.2f), 0);
        }

        protected override void EnterMovement()
        {
            _moveBehavior.Reset();
            _animator.SetTrigger(_runHash);
        }

        protected override void EnterDeath()
        {
            OnDeathCallback?.Invoke(this, true);
            _animator.SetTrigger(_hashDeath);
        }

        protected override void EnterAttack()
        {
            _animator.SetTrigger(_attackHash);
        }

        #endregion

        #region AnimEvent

        private void OnPunch()
        {
            if(CurrentState == HumanState.Death) return;

            _attackBehavior.Attack(_damagableTarget);
        }

        private void OnDeath()
        {
            OnDeathCallback?.Invoke(this, false);
        }

        #endregion

        public override void Command(ICommand command)
        {
            switch (command)
            {
                case Idle idle:
                    CurrentState = HumanState.Idle;
                    break;
                case Move move:
                    CurrentState = HumanState.Movement;
                    break;
                default:
                    CurrentState = HumanState.Movement;
                    break;
            }
        }

        protected override void Tick()
        {
            switch (CurrentState)
            {
                case HumanState.Attack:
                    if (CheckDistance(_currentTarget.position, _property.ReachedDistance) == false || _currentTarget.TryGetComponent(out _damagableTarget) == false)
                        CurrentState = HumanState.Movement;
                    break;
                
                case HumanState.Movement:
                    _moveBehavior.Move(_currentTarget.position, _isClimb);
            
                    if (CheckDistance(_currentTarget.position, _property.ReachedDistance) && _currentTarget.TryGetComponent(out _damagableTarget))
                        CurrentState = HumanState.Attack;
                    break;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy(); 
            
            _animatorHelper.OnAnimationPunchEvent -= OnPunch;
            _animatorHelper.OnAnimationDeathEvent -= OnDeath;
        }
    }
}