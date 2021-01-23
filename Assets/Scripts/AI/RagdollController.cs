using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    private void Awake()
    {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            rb.isKinematic = true;
        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = false;
        foreach (GravityObject go in GetComponentsInChildren<GravityObject>())
            go.enabled = false;

    }

    public void EnableRagdoll(bool gravity = true, float mass = -1)
    {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = false;
            rb.useGravity = gravity;
            if (mass > 0)
                rb.mass = (rb.mass / 70f) * mass;
        }
        foreach (Collider col in GetComponentsInChildren<Collider>())
            col.enabled = true;
        foreach (GravityObject go in GetComponentsInChildren<GravityObject>())
            go.enabled = true;
    }
}
