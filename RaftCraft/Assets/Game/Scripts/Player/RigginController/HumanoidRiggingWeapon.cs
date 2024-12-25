using System;
using DG.Tweening;
using Game.Scripts.Core;
using Game.Scripts.Player.WeaponController;
using Game.Scripts.Pool;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Game.Scripts.Player.RigginController
{
    public class HumanoidRiggingWeapon : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("Components")] private Animator _animator;

        [SerializeField, FoldoutGroup("Components")] private MultiAimConstraint[] _spineConstains;

        [SerializeField, FoldoutGroup("Settings"), Range(0f, 1f)] private float _handWeight;

        [SerializeField] private Transform spearPrefab;
        [SerializeField] private GameObject harpoonSpine;
        [SerializeField] private ParticleController bloodParticle;
        [field: SerializeField] public Transform HarpoonHandPosition { get; private set; }

        public bool _isHarpoon;

        private PoolObjects<ParticleController> _bloodParticles;
        private WeaponEntityData _currentWeapon;
        protected readonly int HashWeaponId = Animator.StringToHash("IdWeapon");

        public void SetWeigh(float weight)
        {
            weight = Mathf.Clamp(weight, 0f, 1f);
            _handWeight = weight;
        }

        public void SetCurrentWeaponModel(WeaponEntityData data)
        {
            HideHarpoon();
            
            if (data is { Type: WeaponType.Hand })
            {
                return;
            }

            _currentWeapon = data;
            if (_currentWeapon != null)
            {
                _animator.SetInteger(HashWeaponId, (int)_currentWeapon.Type);
            }
            else
            {
                _animator.SetInteger(HashWeaponId, 0);
            }
        }

        private void HideHarpoon()
        {
            harpoonSpine.SetActive(false);
        }

        public void SetHarpoon(bool value)
        {
            _isHarpoon = value;

            harpoonSpine.SetActive(!value);
            HarpoonHandPosition.gameObject.SetActive(value);
        }

        public void SetWeightSpine(float weight)
        {
            weight = Mathf.Clamp(weight, 0f, 1f);
            for (var i = 0; i < _spineConstains.Length; i++)
            {
                _spineConstains[i].weight = weight;
            }
        }

        private void Start()
        {
            _bloodParticles = new PoolObjects<ParticleController>(bloodParticle, new GameObject("BloodParticles").transform, 10);
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (_animator == null)
            {
                return;
            }

            if (_isHarpoon == false && _currentWeapon == null)
            {
                return;
            }

            if (_currentWeapon != null)
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _handWeight);
                _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _handWeight);
                _animator.SetIKPosition(AvatarIKGoal.RightHand, _currentWeapon.Behaviour.PointRightArm().position);
                _animator.SetIKRotation(AvatarIKGoal.RightHand, _currentWeapon.Behaviour.PointRightArm().rotation);

                _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _handWeight);
                _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _handWeight);

                _animator.SetIKPosition(AvatarIKGoal.LeftHand, _currentWeapon.Behaviour.PointLeftArm().position);
                _animator.SetIKRotation(AvatarIKGoal.LeftHand, _currentWeapon.Behaviour.PointLeftArm().rotation);
            }

            if (_isHarpoon)
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _handWeight);
                _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _handWeight);
                _animator.SetIKPosition(AvatarIKGoal.RightHand, HarpoonHandPosition.position);
                _animator.SetIKRotation(AvatarIKGoal.RightHand, HarpoonHandPosition.rotation);
            }
        }

        //TODO: Вынести в другое место
        public void HarpoonShot(Transform target, float delay)
        {
            var position = HarpoonHandPosition.position;
            position.z -= 1;
            var spear = Instantiate(spearPrefab, position, Quaternion.identity);
            spear.LookAt(target);
            spear.DOMove(target.position, delay).OnKill(() =>
            {
                var blood = _bloodParticles.GetFree();
                var spearPosition = spear.position;
                spearPosition.z += 1;
                blood.transform.position = spearPosition;
                blood.gameObject.SetActive(true);
                Destroy(spear.gameObject);
            });
        }
    }
}