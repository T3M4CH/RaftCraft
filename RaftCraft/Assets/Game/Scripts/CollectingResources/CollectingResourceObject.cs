using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Scripts.ResourceController.Enums;
using Game.Scripts.ResourceController.Interfaces;
using UnityEngine;

namespace Game.Scripts.CollectingResources
{
    public abstract class CollectingResourceObject : MonoBehaviour
    {
        public event Action<int> OnChangeCount;

        [SerializeField] private GeneralTrigger _generalTrigger;
        [field: SerializeField] public SphereCollider Collider { get; private set; }
        [field: SerializeField] public ObjectMoveToTarget MoveToTarget { get; private set; }
        public event Action<CollectingResourceObject> OnCollection = _ => { };

        public IEnumerable<(EResourceType, int)> Resources;

        private EResourceType _resourceType;
        private bool _isUsed;

        public EResourceType ResourceType
        {
            get => _resourceType;

            set
            {
                _resourceType = value;
                UpdateType(_resourceType);
            }
        }

        private int _count;

        public int Count
        {
            get => _count;

            private set
            {
                _count = value;
                OnChangeCount?.Invoke(_count);
            }
        }

        protected IGameResourceService _resourceService;

        public void FireCollectEvent()
        {
            OnCollection.Invoke(this);
        }

        public void Initialize(IGameResourceService resourceService, IEnumerable<(EResourceType, int)> resources)
        {
            _resourceService = resourceService;

            Count = 0;
            Resources = resources;
            foreach (var resource in Resources)
            {
                ResourceType = resource.Item1;
                Count += resource.Item2;
            }
        }

        protected virtual void OnEnable()
        {
            Collider.radius = 1f;
            _isUsed = false;
        }

        public virtual void UpdateType(EResourceType resourceType)
        {
        }

        protected void UpdateCount(int count)
        {
            Count = count;
        }

        public void CollideArea(Transform coll)
        {
            if (_isUsed) return;
            _isUsed = true;
            if (IsAbsorbent)
            {
                var position = coll.position;
                position.y += 1.5f;
                transform.DOMove(position, 0.2f).SetEase(Ease.InExpo).SetLink(coll.gameObject).OnKill(Collect);
            }
            else
            {
                Collect();
            }
        }

        public virtual void Collect()
        {
            OnCollection.Invoke(this);
        }

        private void Start()
        {
            _generalTrigger.OnTriggerEnterAction += CollideArea;
        }

        private void OnDestroy()
        {
            OnCollection = null;
            _generalTrigger.OnTriggerEnterAction -= CollideArea;
        }

        private bool _isAbsorbent;

        public bool IsAbsorbent
        {
            get => _isAbsorbent;
            set
            {
                _isAbsorbent = value;
                Collider.radius *= 2.5f;
            }
        }
    }
}