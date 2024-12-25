using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class MonoRagdollFragment : MonoBehaviour
{
    [SerializeField] private Collider ragdollCollider;
    [SerializeField] protected Rigidbody rigidBody;
    [SerializeField] private float _impulse = 7f;

    private bool _useGravity;
    private float _customGravity;
    private int _hazardLayerMask;
    private GameObject _effect;
    

    public void EnableCollider()
    {
        ragdollCollider.enabled = true;
    }

    public void SetGravity(float value)
    {
        _customGravity = value;
        _useGravity = true;
    }

    private void GravityUpdate()
    {
        if (_useGravity)
        {
            rigidBody.AddForce(Vector3.down * (_customGravity * rigidBody.mass));
        }
    }

    private void FixedUpdate()
    {
        GravityUpdate();
    }

    public void Activate(Vector3 force)
    {
        if (IsUsed) return;

        EnableCollider();

        IsUsed = true;
        rigidBody.isKinematic = false;

        if (force == Vector3.zero) return;
        rigidBody.AddForce(force * (_impulse * rigidBody.mass), ForceMode.Impulse);
    }

    [Button]
    private void GetColliders()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.isKinematic = true;
        ragdollCollider = GetComponent<Collider>();
        ragdollCollider.enabled = false;
    }
    
    protected bool IsUsed { get; set; }
}