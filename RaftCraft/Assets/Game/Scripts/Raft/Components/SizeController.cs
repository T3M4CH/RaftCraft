using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Scripts.Raft.Components
{
    public class SizeController : MonoBehaviour
    {
        [SerializeField, FoldoutGroup("Settings")]
        private Transform _leftPoint;

        [SerializeField, FoldoutGroup("Settings")]
        private Transform _rightPoint;

        [SerializeField, FoldoutGroup("Settings")]
        private List<Transform> _tiles = new List<Transform>();

        [SerializeField, FoldoutGroup("Settings")]
        private List<Transform> _leftRopes = new List<Transform>();
        
        [SerializeField, FoldoutGroup("Settings")]
        private List<Transform> _rightRopes = new List<Transform>();

        [SerializeField, FoldoutGroup("Settings")]
        private List<BoxCollider> _colliders = new List<BoxCollider>();
        
        [field: SerializeField, FoldoutGroup("Controller"), OnValueChanged("LeftOffsetChange"), Range(0f, 1f)]
        public float LeftOffset { get; private set; }
        
        [field: SerializeField, FoldoutGroup("Controller"), OnValueChanged("RightOffsetChange"), Range(0f, 1f)]
        public float RightOffset { get; private set; }

        [SerializeField, FoldoutGroup("Default Value")]
        private float _maxDistance;

        [SerializeField, FoldoutGroup("Default Value")]
        private float _sizeCollider;

        [SerializeField, FoldoutGroup("Default Value")]
        private float _offsetCollider;
        
        [SerializeField, FoldoutGroup("Default Value")]
        private float _ropeSizeMin;
        
        [SerializeField, FoldoutGroup("Default Value")]
        private float _ropeSizeMax;

        public void SetLeftSize(float size)
        {
            LeftOffset = Mathf.Clamp(size, 0f, 1f);
            LeftOffsetChange();
        }

        public void SetRightSize(float size)
        {
            RightOffset = Mathf.Clamp(size, 0f, 1f);
            RightOffsetChange();
        }
        
        private void LeftOffsetChange()
        {
            var leftDistance = -(_maxDistance * Mathf.Clamp(LeftOffset, 0.38f, 1f));
            _leftPoint.localPosition = new Vector3(leftDistance, 0f, 0f);
            ChangeTiles();
            ChangeColliderSize();
            ChangeColliderCenter();
            foreach (var rope in _leftRopes)
            {
                rope.localPosition = new Vector3(-Mathf.Lerp(_ropeSizeMin, _ropeSizeMax, Mathf.Clamp(LeftOffset, 0.38f, 1f)),
                    rope.localPosition.y, rope.localPosition.z);
                rope.localScale = new Vector3(LeftOffset, 1f, 1f);
            }
        }
        
        private void RightOffsetChange()
        {
            var leftDistance = (_maxDistance * Mathf.Clamp(RightOffset, 0.38f, 1f));
            _rightPoint.localPosition = new Vector3(leftDistance, 0f, 0f);
            ChangeTiles();
            ChangeColliderSize();
            ChangeColliderCenter();
            foreach (var rope in _rightRopes)
            {
                rope.localPosition = new Vector3(Mathf.Lerp(_ropeSizeMin, _ropeSizeMax, Mathf.Clamp(RightOffset, 0.38f, 1f)),
                    rope.localPosition.y, rope.localPosition.z);
                rope.localScale = new Vector3(RightOffset, 1f, 1f);
            }
        }

        private void ChangeColliderSize()
        {
            foreach (var collider in _colliders)
            {
                var left = (_maxDistance - (_maxDistance * Mathf.Clamp(LeftOffset, 0.38f, 1f))) * 0.95f;
                var right = (_maxDistance - (_maxDistance * Mathf.Clamp(RightOffset, 0.38f, 1f))) * 0.95f;
                var change = _sizeCollider - (left + right);
                collider.size = new Vector3(change, collider.size.y, collider.size.z);
            }
        }

        private void ChangeColliderCenter()
        {
            foreach (var collider in _colliders)
            {
                var left = (_offsetCollider - (_offsetCollider * Mathf.Clamp(LeftOffset, 0.1f, 1f)));
                var right = -(_offsetCollider - (_offsetCollider * Mathf.Clamp(RightOffset, 0.1f, 1f)));
                collider.center = new Vector3(left + right, 0f, 0f);
            }
        }

        private void ChangeTiles()
        {
            foreach (var tile in _tiles)
            {
                tile.gameObject.SetActive(HaveActive(tile.transform.localPosition));
            }
        }

        private bool HaveActive(Vector3 position)
        {
            return position.x >= -(_maxDistance * Mathf.Clamp(LeftOffset, 0.38f, 1f)) &&
                   position.x <= (_maxDistance * Mathf.Clamp(RightOffset, 0.38f, 1f));
        }
    }
}
