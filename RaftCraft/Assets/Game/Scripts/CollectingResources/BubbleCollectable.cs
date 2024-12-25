using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

namespace Game.Scripts.CollectingResources
{
    public class BubbleCollectable : MonoBehaviour
    {
        public event Action<BubbleCollectable> OnDestroy;
        
        [SerializeField] private float speed = 2;
        [SerializeField] public TMP_Text countText;
        [SerializeField] private Transform innerTransform;
        [SerializeField] private GameObject bubbleExplosionParticle;

        [SerializeField, FoldoutGroup("Feedback")] private float _durationSpawn;
        [SerializeField, FoldoutGroup("Feedback")] private Ease _easeSpan;
        [SerializeField, FoldoutGroup("Feedback")] private Vector3 _endScale;

        [SerializeField, FoldoutGroup("Feedback")] private float _durationMove;
        [SerializeField, FoldoutGroup("Feedback")] private Ease _easeMove;
        [SerializeField, FoldoutGroup("Feedback")] private float _radiusMove;

        public Vector3 TargetPosition;
        public CollectingResourceObject Resource;

        public void SetItem(CollectingResourceObject resource, float? yPos = null)
        {
            Resource = resource;
            Resource.transform.SetParent(innerTransform);
            Resource.transform.localPosition = Vector3.zero;
            Resource.transform.localScale = Vector3.one / 2f;
            Resource.OnCollection += ResourceOnOnCollection;
            countText.text = $"{Resource.Count}";
            Resource.OnChangeCount += ResourceOnOnChangeCount;
            Resource.Collider.enabled = false;
            transform.localScale = Vector3.zero;
            transform.DOScale(_endScale, _durationSpawn).SetEase(_easeSpan).OnComplete(() => Resource.Collider.enabled = true);
            
            TargetPosition = transform.position;
            if (yPos.HasValue)
            {
                var currentPos = transform.position.y;
                var targetPos = yPos.Value;
                targetPos = Mathf.Min(-targetPos, targetPos);

                TargetPosition.y = targetPos;
                if (currentPos < targetPos)
                {
                    var time = (targetPos - currentPos) / 1.5f;
                    transform.DOMoveY(targetPos, time).OnComplete(() =>
                    {
                        var randomMove = Random.onUnitSphere * Random.Range(1f, _radiusMove);
                        randomMove.z = 0f;
                        transform.DOMove(transform.position + randomMove, _durationMove).SetEase(_easeMove);
                    });
                }
                else
                {
                    var randomMove = Random.onUnitSphere * Random.Range(1f, _radiusMove);
                    randomMove.z = 0f;
                    transform.DOMove(transform.position + randomMove, _durationMove).SetEase(_easeMove);
                }

                return;
            }
            
            var rndMove = Random.onUnitSphere * Random.Range(1f, _radiusMove);
            rndMove.z = 0f;
            transform.DOMove(transform.position + rndMove, _durationMove).SetEase(_easeMove);
        }

        private void ResourceOnOnChangeCount(int count)
        {
            countText.text = $"{count}";
        }

        private void OnEnable()
        {
            bubbleExplosionParticle.transform.SetParent(transform);
            bubbleExplosionParticle.transform.localPosition = Vector3.zero;
        }

        private void ResourceOnOnCollection(CollectingResourceObject resource)
        {
            Debug.Log($"Collectable Bubble: {gameObject.name}");
            Resource.OnCollection -= ResourceOnOnCollection;
            Resource.transform.SetParent(null);
            Resource.gameObject.SetActive(false);
            bubbleExplosionParticle.transform.SetParent(null);
            bubbleExplosionParticle.SetActive(true);
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            OnDestroy?.Invoke(this);
            OnDestroy = null;
            transform.DOKill();
            if (Resource == null)
            {
                return;
            }

            Resource.OnCollection -= ResourceOnOnCollection;
            Resource.OnChangeCount -= ResourceOnOnChangeCount;
        }
    }
}