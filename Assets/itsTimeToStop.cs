using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itsTimeToStop : MonoBehaviour
{
    private Vector3 _force = new Vector3(-5.0f, 0.0f, 0.0f);

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        Debug.Log(rb);

        if (rb == null) //test if player walked over the pressure plate
        {
            rb = other.GetComponentInParent<Rigidbody>();
        }

        if (rb != null)
        {
            //rb.AddForce(_force);
            rb.velocity = _force;
        }


        //check if oxyGen tank was picked up : do shizzle

    }

}
