using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static GravityObject findGravityObject(Collider collider)
    {
        GravityObject go = collider.GetComponent<GravityObject>();
        if (!go) go = collider.GetComponentInParent<GravityObject>();
        return go;
    }

    public static Rigidbody findRigidbody(Collider collider)
    {
        Rigidbody rb = collider.GetComponent<Rigidbody>();
        if (!rb) rb = collider.GetComponentInParent<Rigidbody>();
        return rb;
    }
}
