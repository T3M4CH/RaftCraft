using System;
using DG.Tweening;
using Game.Scripts.Player;
using Game.Scripts.Player.HeroPumping.Enums;
using Game.Scripts.Player.PlayerController.PlayerStates;
using Game.Scripts.Player.RigginController;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.NPC.Fish.Systems
{
    public class FishingSystem : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("Components")]
        private EntityPlayer _player;

        [SerializeField, FoldoutGroup("Settings Feedback")]
        private float _durationMove;

        [SerializeField, FoldoutGroup("Settings Feedback")]
        private Ease _easeMove;

        [SerializeField] private HumanoidRiggingWeapon humanoidRiggingWeapon;

        private BaseFish _targetBaseFish;

        public BaseFish TargetBaseFish
        {
            get => _targetBaseFish;

            private set
            {
                if (value == null)
                {
                    if (_targetBaseFish != null)
                    {
                        _targetBaseFish.ResetProgress();
                        _targetBaseFish.ComponentsFish.Bar.SetStateProgressBar(false);
                        _targetBaseFish = value;
                        return;
                    }

                    _targetBaseFish = null;
                    return;
                }

                if (_targetBaseFish == null)
                {
                    _targetBaseFish = value;
                    _targetBaseFish.ComponentsFish.Bar.SetStateProgressBar(true);
                }
                else
                {
                    if (_targetBaseFish != value)
                    {
                        _targetBaseFish.ComponentsFish.Bar.SetStateProgressBar(false);
                        _targetBaseFish = value;
                        _targetBaseFish.ComponentsFish.Bar.SetStateProgressBar(true);
                    }
                }
            }
        }

        private float SpeedFishing => _player.PlayerSettings.GetLevel(EPlayerUpgradeType.FishLevel) * 0.5f;

        private bool _haveAttack;
        private Transform _transform;
        private bool _firstAttack;

        private void OnEnable()
        {
            _player.Weapon.Collision.OnTriggerEnterEvent += CollisionOnOnTriggerEnterEvent;
            _player.Weapon.Collision.OnTriggerStayEvent += CollisionOnOnTriggerEnterEvent;
            _player.Weapon.Collision.OnTriggerExitEvent += CollisionOnOnTriggerExitEvent;
        }

        private void CollisionOnOnTriggerExitEvent(Collider collider)
        {
            if (collider.TryGetComponent<BaseFish>(out var fish) == false)
            {
                return;
            }

            fish.ComponentsFish.Bar.SetStateLevelBar(true);
            fish.ComponentsFish.Bar.SetLockSprite(false);


            if (TargetBaseFish == fish)
            {
                humanoidRiggingWeapon.SetWeigh(0);
                humanoidRiggingWeapon.SetHarpoon(false);
                TargetBaseFish = null;
            }
        }

        public bool HaveAttackFish()
        {
            if (TargetBaseFish == null)
            {
                return false;
            }

            return TargetBaseFish.Progress <= 0f;
        }

        private void CollisionOnOnTriggerEnterEvent(Collider collider)
        {
            if (_player.StateMachine.GetState<PlayerWaterState>().IsActive == false)
            {
                return;
            }

            if (collider.TryGetComponent<BaseFish>(out var fish) == false)
            {
                return;
            }

            if (TargetBaseFish != null)
            {
                return;
            }

            if (_player.PlayerSettings.GetLevel(EPlayerUpgradeType.FishLevel) < fish.Level)
            {
                return;
            }

            if (fish.HaveProgress() == false)
            {
                return;
            }

            humanoidRiggingWeapon.SetHarpoon(true);
            humanoidRiggingWeapon.SetWeigh(1);
            TargetBaseFish = fish;
            TargetBaseFish.IsTargetFish = true;
            fish.ComponentsFish.Bar.TryShowLevelBar();
        }

        private void Update()
        {
            if (_player.StateMachine.GetState<PlayerWaterState>().IsActive == false)
            {
                return;
            }

            if (TargetBaseFish == null)
            {
                if (_player.DayService.CurrentDay >= 6)
                {
                    humanoidRiggingWeapon.SetWeigh(0);
                    humanoidRiggingWeapon.SetHarpoon(false);
                }

                _firstAttack = false;
                return;
            }

            if (_player.Space != LocationSpace.Water)
            {
                return;
            }

            if (_haveAttack)
            {
                return;
            }

            var fishTransform = TargetBaseFish.transform;
            humanoidRiggingWeapon.HarpoonHandPosition.LookAt(fishTransform);
            TargetBaseFish.RemoveProgress(Time.smoothDeltaTime * SpeedFishing);

            // if (targetBaseFish.Progress <= 0.5f && !_firstAttack)
            // {
            //     _firstAttack = true;
            //     humanoidRiggingWeapon.HarpoonShot(targetBaseFish.transform, 0.1f);
            // }

            if (TargetBaseFish.Progress <= 0f)
            {
                _haveAttack = true;
                humanoidRiggingWeapon.HarpoonShot(TargetBaseFish.transform, 0.1f);
                TargetBaseFish.enabled = false;
                TargetBaseFish.ComponentsFish.Bar.SetStateProgressBar(false);
                _player.Weapon.AttackComponent.StartAttack();
                TargetBaseFish.ComponentsFish.MoveToTarget.OnCollection += MoveToTargetOnOnCollection;
                TargetBaseFish.ComponentsFish.MoveToTarget.SetTarget(_player.Hips, Vector3.zero);
            }
        }

        private void Start()
        {
            _transform = transform;
        }

        private void MoveToTargetOnOnCollection(BaseFish baseFish)
        {
            if (baseFish)
            {
                baseFish.ComponentsFish.MoveToTarget.OnCollection -= MoveToTargetOnOnCollection;
                baseFish.TakeDamage(1, transform.position);
            }

            TargetBaseFish = null;
            _haveAttack = false;
        }

        private void OnDisable()
        {
            _player.Weapon.Collision.OnTriggerEnterEvent -= CollisionOnOnTriggerEnterEvent;
            _player.Weapon.Collision.OnTriggerStayEvent -= CollisionOnOnTriggerEnterEvent;
            _player.Weapon.Collision.OnTriggerExitEvent -= CollisionOnOnTriggerExitEvent;
            if (TargetBaseFish != null)
            {
                TargetBaseFish.ComponentsFish.MoveToTarget.OnCollection -= MoveToTargetOnOnCollection;
            }
        }
    }
}