using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityReset : MonoBehaviour
{

    private void OnTriggerExit(Collider other)
    {
        GravityObject go = Utils.FindGravityObject(other);
        Rigidbody rb = go.GetRB();
        rb.velocity = rb.velocity / 2;
    }
}
