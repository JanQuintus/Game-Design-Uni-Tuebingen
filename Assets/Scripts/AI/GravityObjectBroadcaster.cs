using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityObjectBroadcaster : MonoBehaviour
{
    [SerializeField] private Transform hips;
    [SerializeField] private Rigidbody hipsRB;
    [SerializeField] GravityObject parentGravity;
    [SerializeField] private Transform mainCollider;
    [SerializeField] private float baseMass = 70f;

    private List<Rigidbody> _childrenRBs = new List<Rigidbody>();
 
    private void Awake()
    {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            if(rb != hipsRB) _childrenRBs.Add(rb);

        hipsRB.mass = baseMass;
        hipsRB.useGravity = true;
        hipsRB.isKinematic = false;

        foreach (Rigidbody rb in _childrenRBs)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.mass = (rb.mass / baseMass) * hipsRB.mass;
        }
        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = true;
    }

    private void FixedUpdate()
    {
        mainCollider.position = hips.position;
        mainCollider.rotation = hips.rotation;

        foreach (Rigidbody rb in _childrenRBs)
        {
            rb.useGravity = hipsRB.useGravity;
            rb.mass = (rb.mass / baseMass) * hipsRB.mass;

            if (hipsRB.useGravity)
                rb.AddForce(parentGravity.GetLocalGravity(), ForceMode.Acceleration);
        }
        baseMass = hipsRB.mass;

    }
}
