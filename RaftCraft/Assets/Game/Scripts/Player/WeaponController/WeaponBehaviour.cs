using System;
using DG.Tweening;
using Game.Scripts.Core.Interface;
using Game.Scripts.Player.EntityGame;
using Game.Scripts.Player.WeaponController.BulletSystem;
using Game.Scripts.Player.WeaponController.WeaponsData;
using Game.Scripts.Pool;
using GTapSoundManager.SoundManager;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Player.WeaponController
{
    public abstract class WeaponBehaviour : MonoBehaviour, IGameObserver<WeaponUpgrade>
    {
        [System.Serializable]
        public struct WeaponComponents
        {
            [field: SerializeField] public LineRenderer Line { get; private set; }
            [field: SerializeField] public Transform PointSpawnBullet { get; private set; }
            [field: SerializeField] public Transform PointRightHand { get; private set; }
            [field: SerializeField] public Transform PointLeftHand { get; private set; }
        }

        [field: SerializeField] public WeaponId Id { get; private set; }
        [field: SerializeField] public WeaponComponents Components { get; private set; }
        [field: SerializeField] public WeaponsDataSettings Data { get; private set; }
        [SerializeField, FoldoutGroup("Settings")] protected float _distanceAutoAttack;
        [SerializeField, FoldoutGroup("Settings")] private Transform _holder;
        [SerializeField, FoldoutGroup("Sound Effect")] private SoundAsset _assetShot;
        [SerializeField, FoldoutGroup("Visual Effect Shot")] private GameObject _effectShot;
        [SerializeField, FoldoutGroup("Feedback Shot")] private float _durationShot;
        [SerializeField, FoldoutGroup("Feedback Shot")] private Ease _easeShot;
        [SerializeField, FoldoutGroup("Feedback Shot")] private Vector3 _positionShot;

        private PoolObjects<Bullet> _poolBullets;
        private Vector3 _startPosition;
        private int _level;

        public virtual void Initialize()
        {
            Components.Line.positionCount = 2;
        }

        protected Bullet GetBullet()
        {
            _poolBullets ??= new PoolObjects<Bullet>(Data.PrefabBullet, transform, 10);
            return _poolBullets.GetFree();
        }

        protected virtual void Start()
        {
            _startPosition = _holder.localPosition;

            Initialize();
        }

        public virtual void Hide()
        {
            _effectShot.SetActive(false);
        }

        private void Update()
        {
            Components.Line.SetPosition(0, Components.PointSpawnBullet.position);
            Components.Line.SetPosition(1, Components.PointSpawnBullet.position + Components.PointSpawnBullet.forward * DistanceAttack());
        }

        public virtual Transform PointRightArm()
        {
            return Components.PointRightHand;
        }

        public virtual Transform PointLeftArm()
        {
            return Components.PointLeftHand;
        }

        public virtual float Damage()
        {
            if (Data == null)
            {
                return 0f;
            }

            return Data.Damage(_level);
        }

        public virtual float DistanceAttack()
        {
            if (Data == null)
            {
                return 0f;
            }

            return Data.DistanceAttack(_level);
        }

        public virtual float TimeReload()
        {
            if (Data == null)
            {
                return 0f;
            }

            return Data.TimeReload(_level);
        }

        public virtual float Spread()
        {
            if (Data == null)
            {
                return 0f;
            }

            return Data.Spread(_level);
        }

        public virtual float ChangeCritical()
        {
            if (Data == null)
            {
                return 0f;
            }

            return Data.ChangeCritical(_level);
        }

        protected Vector3 SpreadForward(Vector3 forward)
        {
            forward.y += Random.Range(-1f, 1f) * Spread();
            return forward;
        }

        public virtual void Shot(EntityType target, Entity targetEntity)
        {
            if (_assetShot != null)
            {
                _assetShot.Play(Random.Range(0.95f, 1.05f));
            }

            _effectShot.SetActive(false);
            _effectShot.SetActive(true);
            _holder.DOKill();
            _holder.DOLocalMove(_startPosition + _positionShot, _durationShot).SetEase(_easeShot).OnComplete(() => { _holder.DOLocalMove(_startPosition, _durationShot).SetEase(_easeShot); });
        }

        public void PerformNotify(WeaponUpgrade data)
        {
            if (data.Id != Id)
            {
                return;
            }

            _level = data.Level;

            Debug.Log($"Weapon Upgrade! Level: {_level} Damage: {Damage()}");
        }

        private void OnDisable()
        {
            _holder.DOKill();
        }
    }
}