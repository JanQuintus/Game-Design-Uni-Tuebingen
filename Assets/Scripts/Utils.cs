using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static GravityObject FindGravityObject(Collider collider)
    {
        GravityObject go = collider.GetComponent<GravityObject>();
        if (!go) go = collider.GetComponentInParent<GravityObject>();
        return go;
    }

    public static void SetMaterialPropertyFloat(Renderer input, int propertyID, float value)
    {
        Renderer[] children;
        children = input.GetComponentsInChildren<Renderer>();
        input.material.SetFloat(propertyID, value);
        foreach (Renderer rend in children)
        {
            foreach(Material material in rend.materials)
                material.SetFloat(propertyID, value);
        }
    }

    public static Health FindHealth(Collider collider)
    {
        Health go = collider.GetComponent<Health>();
        if (!go) go = collider.GetComponentInParent<Health>();
        return go;
    }
}
