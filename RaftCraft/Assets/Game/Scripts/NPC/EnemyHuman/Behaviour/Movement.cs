using Game.GameBalanceCore.scripts;
using Game.GameBalanceCore.Scripts.BalanceValue;
using MoreMountains.Tools;
using UnityEngine;

namespace Game.Scripts.NPC
{
    public class Movement : IMovable
    {
        private enum Mode
        {
            Run,
            Climb,
        }
        
        private readonly float _maxSpeedMove;
        private readonly float _speedRotate;
        private readonly Transform _transform;
        private readonly Rigidbody _rigidbody;
        private readonly Collider _collider;
        private readonly Animator _animator;
        private readonly int _hashMoveMode = Animator.StringToHash("MoveAnim");

        private float _currentSpeed;
        private Mode _mode;

        private Mode CurrentMode
        {
            get => _mode;
            set
            {
                if(_mode == value) return;
                
                _mode = value;
                switch (value)
                {
                    case Mode.Run:
                        _rigidbody.isKinematic = false;
                        _collider.isTrigger = false;
                        _animator.SetFloat(_hashMoveMode, 0f);
                        break;
                    case Mode.Climb:
                        _rigidbody.isKinematic = true;
                        _collider.isTrigger = true;
                        _animator.SetFloat(_hashMoveMode, 1f);
                        break;
                }
            }
        }
        
        public Movement(Transform transform, Rigidbody rigidbody, Collider collider, Animator animator, float speedMove, float speedRotate)
        {
            _transform = transform;
            _rigidbody = rigidbody;
            _collider = collider;
            _animator = animator;
            _maxSpeedMove = speedMove;
            _speedRotate = speedRotate;
        }
        
        public void Move(Vector3 target, bool isClimb)
        {
            target.MMSetZ(_transform.position.z);
            CurrentMode = isClimb ? Mode.Climb : Mode.Run;
            
            if (CurrentMode == Mode.Run)
            {
                Rotate(target - _transform.position);
            }
            else
            {
                _transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
            
            var delta = Time.deltaTime;
            _currentSpeed = Mathf.Lerp(_currentSpeed, _maxSpeedMove, delta * 4f);
            
            _transform.position = Vector3.MoveTowards(_transform.position, target, Time.deltaTime * GameBalance.Instance.GetBalanceValue(_currentSpeed, TypeValueBalance.PirateSpeedMove));
        }

        public void Reset()
        {
            _currentSpeed = 0f;
        }
        
        private void Rotate(Vector3 direction)
        {
            if (direction == Vector3.zero) return;

            direction.y = 0f;
            
            var targetRotation = Quaternion.LookRotation(direction);
            _transform.rotation = Quaternion.RotateTowards(_transform.rotation, targetRotation, _speedRotate * Time.deltaTime);
        }
    }
}