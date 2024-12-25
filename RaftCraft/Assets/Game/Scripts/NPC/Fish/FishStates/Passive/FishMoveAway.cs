using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts.Player.EntityGame;
using Game.Scripts.Player.StateMachine;
using Pathfinding;
using UnityEngine;

namespace Game.Scripts.NPC.Fish.FishStates.Passive
{
    public class FishMoveAway : FishState
    {
        private List<Vector3> _path = new List<Vector3>();
        private Transform _away;

        private bool _isTargetFish;
        private float _currentTime;
        private float _boostSpeed = 1;
        private AnimationCurve _curveBoost;
        
        private const float Delay = 1f;

        public FishMoveAway(EntityStateMachine stateMachine, Entity entity, AnimationCurve curveBoost) : base(stateMachine, entity)
        {
            _curveBoost = curveBoost;
        }

        public override void Enter()
        {
            base.Enter();
            _currentTime = 0f;
            //TODO: Что-то отключил
            //CurrentAnimator.SetTrigger(RunHash);
            CurrentAnimator.SetFloat(HashSpeedRun, Random.Range(2f, 2.5f));
            currentBaseFish.ComponentsFish.Seeker.pathCallback += PathCallback;
        }

        public void SetAway(Transform away)
        {
            _boostSpeed = 1;
            _isTargetFish = currentBaseFish.IsTargetFish;

            _away = away;

            var fishPosition = currentBaseFish.Components.ParentObject.position;
            var direction = (fishPosition - _away.position).normalized;
            var targetPath = fishPosition + (direction * 5f);

            if (!_isTargetFish)
            {
                var randomDirection = Random.onUnitSphere;
                randomDirection.z = 0;
                targetPath = fishPosition + (randomDirection * 7f);
            }

            if (currentBaseFish.YSpawnPosition > currentBaseFish.YBarrier)
            {
                if(targetPath.y < currentBaseFish.YBarrier + 2)
                {
                    var diff = targetPath.y - (currentBaseFish.YBarrier + 2);

                    targetPath.x += Random.Range(diff, -diff);
                }

                targetPath.y = Mathf.Max(targetPath.y, currentBaseFish.YBarrier + 2);
            }
            else
            {
                targetPath.y = Mathf.Min(targetPath.y, currentBaseFish.YBarrier - 2);
            }

            currentBaseFish.ComponentsFish.Seeker.StartPath(currentBaseFish.Components.ParentObject.position, targetPath);
        }

        private void PathCallback(Path path)
        {
            _path = path.vectorPath;
        }

        public override void Exit()
        {
            base.Exit();
            currentBaseFish.ComponentsFish.Seeker.pathCallback += PathCallback;
            CurrentAnimator.SetFloat(HashSpeedRun, 1);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Move();
        }

        private void Move()
        {
            if (HaveUpdateNextPoint())
            {
                return;
            }

            var target = (GetNextPoint() - currentBaseFish.Components.ParentObject.position).normalized;

            // if (!_isTargetFish)
            // {
            //     _currentTime -= Time.deltaTime;
            //
            //     switch (_currentTime)
            //     {
            //         case < 0:
            //             _boostSpeed = 1;
            //             _currentTime = Delay;
            //             break;
            //         case < 0.45f:
            //             var ratio = Mathf.InverseLerp(0.3f, 0, _currentTime);
            //             _boostSpeed = Mathf.Lerp(3.5f, 0.5f, ratio);
            //             break;
            //     }
            // }
            if (!_isTargetFish)
            {
                _currentTime -= Time.deltaTime;

                switch (_currentTime)
                {
                    case < 0:
                        _boostSpeed = 1;
                        _currentTime = Delay;
                        break;
                    case < 0.45f:
                        var ratio = Mathf.InverseLerp(0.3f, 0, _currentTime);
                        _boostSpeed = Mathf.Lerp(3.5f, 0.5f, ratio);
                        break;
                }
            }

            currentBaseFish.ComponentsFish.Rb.MovePosition(currentBaseFish.Components.ParentObject.position + (target * (Time.fixedDeltaTime * currentBaseFish.Data.SpeedBoostMove * _boostSpeed)));
            
           
            Rotate(target);

            if (HaveCompletePath())
            {
                if (DistanceTarget() <= 6f)
                {
                    SetAway(_away);
                }
                else
                {
                    stateMachine.SetState<FishIdle>();
                }
            }
        }

        private float DistanceTarget()
        {
            if (_away == null)
            {
                return float.MaxValue;
            }

            return Vector3.Distance(currentBaseFish.Components.ParentObject.position, _away.position);
        }

        private void Rotate(Vector3 direction)
        {
            if (direction == Vector3.zero)
            {
                return;
            }
            var targetRotate = Quaternion.LookRotation(direction);
            Entity.Components.ModelObject.rotation = Quaternion.RotateTowards(Entity.Components.ModelObject.rotation, targetRotate,
                180f * Time.fixedDeltaTime);
        }
        
        private Vector3 GetNextPoint()
        {
            if (_path == null || _path.Count == 0)
            {
                return currentBaseFish.Components.ParentObject.position;
            }

            return _path[0];
        }

        private bool HaveCompletePath()
        {
            return _path.Count == 0;
        }

        private bool HaveUpdateNextPoint()
        {
            if (_path == null || _path.Count == 0)
            {
                return false;
            }

            if (!(Vector3.Distance(currentBaseFish.Components.ParentObject.position, _path[0]) <=
                  currentBaseFish.Data.DistanceUpdatePatch)) return false;
            _path.RemoveAt(0);
            return true;
        }
    }
}