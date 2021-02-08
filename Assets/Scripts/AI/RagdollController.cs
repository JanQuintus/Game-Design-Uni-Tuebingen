using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private Transform hips;
    [SerializeField] private Rigidbody hipsRB;

    private GravityObject _parentGravity;

    private bool _enabled = false;
    private List<Rigidbody> _childrenRBs = new List<Rigidbody>();
    private Transform _mainCollider;
    private float _baseMass = 70f;

    private void Awake()
    {
        _parentGravity = GetComponentInParent<GravityObject>();

        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = true;
            if(rb != hipsRB) _childrenRBs.Add(rb);
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
            rb.useGravity = hipsRB.useGravity;
            rb.mass = (rb.mass / _baseMass) * hipsRB.mass;

            if (hipsRB.useGravity)
                rb.AddForce(_parentGravity.GetLocalGravity(), ForceMode.Acceleration);
        }
        _baseMass = hipsRB.mass;

    }

    public void EnableRagdoll(Transform mainCollider, Rigidbody copyRB)
    {
        _mainCollider = mainCollider;
        hipsRB.mass = copyRB.mass;
        hipsRB.useGravity = copyRB.useGravity;
        hipsRB.isKinematic = false;

        foreach (Rigidbody rb in _childrenRBs)
        {
            rb.isKinematic = false;
            rb.useGravity = copyRB.useGravity;
            rb.mass = (rb.mass / _baseMass) * hipsRB.mass;
        }
        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = true;

        _baseMass = copyRB.mass;


        _enabled = true;
    }

    public Rigidbody GetHipsRB() => hipsRB;
}
