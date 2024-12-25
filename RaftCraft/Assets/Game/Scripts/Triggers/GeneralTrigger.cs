using System;
using UnityEngine;
using UnityEngine.Events;

public class GeneralTrigger : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private UnityEvent _enterTriggerAction;
    [SerializeField] private UnityEvent _exitTriggerAction;

    public event Action<Transform> OnTriggerEnterAction = _ => {};
    
    private void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer) == _layerMask)
        {
            _enterTriggerAction?.Invoke();
            OnTriggerEnterAction.Invoke(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((1 << other.gameObject.layer) == _layerMask)
        {
            _exitTriggerAction?.Invoke();
        }
    }
}
