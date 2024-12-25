using System;
using Game.Scripts.NPC.Fish;
using UnityEngine;

public class ObjectMoveToTarget : MonoBehaviour
{
    public event Action<BaseFish> OnCollection;
    
    [SerializeField] private float _speedMove;
    [SerializeField] private BaseFish baseFish;
    [SerializeField] private AnimationCurve _curveMove;
    [SerializeField] private Transform _modelScale;
    
    private Transform _target;
    private float _progress;
    private Vector3 _offset;

    private void OnEnable()
    {
        if (_modelScale != null)
        {
            _modelScale.localScale = Vector3.one;
        }
    }

    public void SetTarget(Transform target, Vector3 offset)
    {
        _progress = 0f;
        _offset = offset;
        _target = target;
    }
    private void FixedUpdate()
    {
        if (_target == null)
        {
            return;
        }

        transform.position = Vector3.Lerp(transform.position, _target.position + _offset, _progress);
        if (_modelScale != null)
        {
            _modelScale.localScale = Vector3.Lerp(_modelScale.localScale, Vector3.zero, _progress);
        }
        _progress += Time.fixedDeltaTime * _speedMove;

        if (Vector3.Distance(transform.position,_target.position + _offset) <= 0.15f)
        {
            if (baseFish != null)
            {
                OnCollection?.Invoke(baseFish);
            }

            if (_modelScale != null)
            {
                _modelScale.localScale = Vector3.one;
            }
            _target = null;
        }
        
    }

    private void OnDisable()
    {
        _target = null;
    }
}
