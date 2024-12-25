using Game.Scripts.Utils;
using UnityEngine;

namespace Game.Scripts.NPC
{
    public class RangeHuman : HumanBase
    {
        #region Fields

        [SerializeField] private Transform _pointSpawnBullet;
        [SerializeField] private float _cooldownShoot = 0.5f;
        [SerializeField] private AnimatorHelperEvent _animatorHelper;
        //[SerializeField] private Bullet _bulletPrefab;

        private float _currentCooldown;
        
        private readonly int _runHash = Animator.StringToHash("MoveBlend");
        private readonly int _attackHash = Animator.StringToHash("Attack");
        private readonly int _idleBlend = Animator.StringToHash("IdleBlend");

        #endregion

        #region Init

        protected override void OnInit()
        {
            _moveBehavior = new Movement(transform, _rigidbody, _collider, _animator, _speedMoveModified, _property.SpeedRotate);
            _attackBehavior = new AttackRiffleRange(_property.Damage, _pointSpawnBullet);

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

        private void OnDeath()
        {
            OnDeathCallback?.Invoke(this, false);
        }
        
        #endregion

        protected override void Tick()
        {
            switch (CurrentState)
            {
                case HumanState.Attack:
                    _currentCooldown += Time.deltaTime;

                    if (_currentCooldown > _cooldownShoot)
                    {
                        _currentCooldown = 0f;
                        _attackBehavior.Attack(_damagableTarget);
                    }
                    
                    var positionTarget = _navigation.Player.transform.position;
                    if (CheckDistance(positionTarget, _property.ReachedDistance) == false)
                        CurrentState = HumanState.Movement;
                    break;
                
                case HumanState.Movement:
                    var target = transform.position;
                    target.x = _currentTarget.position.x;

                    _moveBehavior.Move(target, _isClimb);
            
                    if (CheckDistance(target, _property.ReachedDistance) && _currentTarget.TryGetComponent(out _damagableTarget))
                        CurrentState = HumanState.Attack;
                    break;
            }
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy(); 
            
            _animatorHelper.OnAnimationDeathEvent -= OnDeath;
        }
    }
}