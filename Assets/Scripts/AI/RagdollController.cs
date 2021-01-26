using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private Transform hips; 

    private GravityObject _parentGravity;
    private Rigidbody _parentRB;

    private bool _enabled = false;
    private List<Rigidbody> _childrenRBs = new List<Rigidbody>();
    private float _baseMass = 70f;
    private Transform _mainCollider;

    private void Awake()
    {
        _parentGravity = GetComponentInParent<GravityObject>();
        _parentRB = GetComponentInParent<Rigidbody>();

        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = true;
            _childrenRBs.Add(rb);
        }
        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = false;
    }

    private void FixedUpdate()
    {
        if (!_enabled)
            return;

        _mainCollider.position = hips.position;
        _mainCollider.rotation = hips.rotation;

        foreach (Rigidbody rb in _childrenRBs)
        {
            rb.useGravity = _parentRB.useGravity;
            rb.mass = (rb.mass / _baseMass) * _parentRB.mass;
            _baseMass = _parentRB.mass;
            if(_parentRB.useGravity)
                rb.AddForce(_parentGravity.GetLocalGravity(), ForceMode.Acceleration);
        }
    }

    public void EnableRagdoll(Transform mainCollider)
    {
        _mainCollider = mainCollider;
        foreach (Rigidbody rb in _childrenRBs)
        {
            rb.isKinematic = false;
            rb.useGravity = _parentRB.useGravity;
            rb.mass = (rb.mass / 70f) * _parentRB.mass;
        }
        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = true;

        _enabled = true;
    }
}
