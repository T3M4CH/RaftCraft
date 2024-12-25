using Sirenix.Utilities;
using UnityEngine;
using System;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace Game.Prefabs.NPC.Vendors
{
    public class MonoCaughtFish : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private GameObject[] models;

        private float _radius = 1;
        private float _targetTime;
        private float _currentTime;
        private Vector3 _targetPoint;
        private Vector3 _centerOffset;
        private Vector3 _startPosition;
        private Quaternion _startRotation;
        private Transform _transform;

        public void SetLevel(int level)
        {
            try
            {
                models.ForEach(model => model.SetActive(false));
                models[level].SetActive(true);
                Level = level + 1;
            }
            catch (Exception e)
            {
                Debug.LogError($"Level {level} out of range, {models.Length - 1} is max");
            }
        }

        public bool IsStatic { get; set; }

        public void UpdateRadius(float radius, Vector3 centerPosition)
        {
            _radius = radius;
            _centerOffset = centerPosition;
        }

        private Vector3 GetRandomPoint()
        {
            var randomX = 0f;
            var randomY = 0f;

            do
            {
                randomX = Random.Range(-0.6f, 0.6f);
                randomY = Random.Range(-0.6f, 0.6f);
            } while (randomX + randomY > 1);

            return new Vector3(randomX, randomY, 0) * _radius + _centerOffset;
        }

        private float CalculateTime()
        {
            var distance = Vector3.Distance(_transform.position, _targetPoint);

            return distance / speed;
        }

        private void InitializeNewPoint()
        {
            _transform.DOKill();
            _startPosition = _transform.position;
            _targetPoint = GetRandomPoint();
            _transform.DOLookAt(_targetPoint, 0.3f);
            _targetTime = CalculateTime();
            _currentTime = 0;
        }

        private void Update()
        {
            if(IsStatic) return;
            
            if (Vector3.Distance(_targetPoint, _transform.position) < 0.1f)
            {
                InitializeNewPoint();
            }

            _currentTime += Time.deltaTime;
            _transform.position = Vector3.Lerp(_startPosition, _targetPoint, _currentTime / _targetTime);
        }

        private void Start()
        {
            if(IsStatic) return;
            
            _transform = transform;
            
            InitializeNewPoint();
        }

        private void OnDestroy()
        {
            _transform.DOKill();
        }

        public int Level { get; private set; }
    }
}