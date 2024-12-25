using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Raft
{
    public class TileRaft : MonoBehaviour
    {
        [System.Serializable]
        public class FragmentData
        {
            [field: SerializeField]public Rigidbody Transform { get; private set; }
            [field: SerializeField] public Collider Collider { get; private set; }
            public Vector3 DefaultPosition;

            public FragmentData(Rigidbody point, Collider collider)
            {
                this.Transform = point;
                this.Collider = collider;
            }
        }

        [SerializeField] private UnityEvent OnEnabled;
        [SerializeField] private UnityEvent OnDisabled;
        
        [field: SerializeField] public PartTile Part { get; private set; }
        [field: SerializeField] public List<FragmentData> Fragments { get; private set; }

        [SerializeField] private float _force;
        [SerializeField] private float _durationHeal;
        [SerializeField] private Ease _easeHeal;


        private void Awake()
        {
            foreach (var fragment in Fragments)
            {
                if (fragment.Collider != null)
                {
                    fragment.DefaultPosition = fragment.Transform.transform.localPosition;
                    fragment.Collider.enabled = false;
                }
            }
        }


        public void Explosion(Vector3 direction)
        {
            foreach (var fragment in Fragments)
            {
                fragment.Collider.enabled = true;
                fragment.Transform.isKinematic = false;
                fragment.Transform.transform.DOKill();
                fragment.Transform.AddForce(direction * _force, ForceMode.Impulse);
            }
        }

        public void EnableTile()
        {
            OnEnabled?.Invoke();
        }

        public void DisableTile()
        {
            OnDisabled?.Invoke();
        }
        
        public void Heal()
        {
            var delay = 0f;
            foreach (var fragment in Fragments)
            {
                fragment.Transform.isKinematic = true;
                fragment.Transform.transform.DOKill();
                fragment.Transform.transform.DOLocalMove(fragment.DefaultPosition, _durationHeal).SetEase(_easeHeal).SetDelay(delay);
                fragment.Transform.transform.DOLocalRotate(Vector3.zero, _durationHeal).SetEase(_easeHeal).SetDelay(delay);
                fragment.Collider.enabled = false;
                delay += 0.05f;
            }
        }

#if UNITY_EDITOR
        [Button]
        private void GetFragments()
        {
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent<MeshCollider>(out var collider) == false)
                {
                    collider = child.gameObject.AddComponent<MeshCollider>();
                    collider.convex = true;
                }
                
                if (child.TryGetComponent<Rigidbody>(out var rb) == false)
                {
                    var rigidBody = child.gameObject.AddComponent<Rigidbody>();
                    rigidBody.isKinematic = true;
                    Fragments.Add(new FragmentData(rigidBody, collider)
                    {
                        DefaultPosition = rigidBody.transform.localPosition
                    });
                }
                else
                {
                    rb.isKinematic = true;
                    Fragments.Add(new FragmentData(rb, collider)
                    {
                        DefaultPosition = rb.transform.localPosition
                    });
                }
            }
        }
        
        [Button]
        private void SortParts()
        {
            var list = Fragments.OrderBy(p => p.Transform.transform.localPosition.x);
            Fragments = list.ToList();
            for (var i = 0; i < Fragments.Count; i++)
            {
                Fragments[i].Transform.transform.SetSiblingIndex(i);
            }
        }

        [Button]
        private void ResetPart()
        {
            foreach (var fragment in Fragments)
            {
                fragment.Transform.transform.localPosition = fragment.DefaultPosition;
            }
        }
#endif

        private void OnDestroy()
        {
            foreach (var fragment in Fragments)
            {
                if (fragment == null || fragment.Transform == null)
                {
                    Debug.Log($"{transform.parent.name}:{gameObject.name}");
                    continue;
                }
                fragment.Transform.transform.DOKill();
            }
        }
    }
}
