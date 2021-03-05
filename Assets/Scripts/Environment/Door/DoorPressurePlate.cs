using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPressurePlate : MonoBehaviour
{
    [SerializeField] private Door door;
    [SerializeField] private int minWeightToPress = 50;

    private int _shader_Mass = Shader.PropertyToID("objmass");

    private void Awake()
    {
        gameObject.GetComponent<Renderer>().material.SetFloat(_shader_Mass, minWeightToPress);
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb == null) //test if player walked over the pressure plate
        {
            rb = other.GetComponentInParent<Rigidbody>();
        }

        if (rb != null) 
        {
            if (rb.mass >= minWeightToPress)
            {
                door.OpenDoor();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb == null) //test if player walked over the pressure plate
        {
            rb = other.GetComponentInParent<Rigidbody>();
        }

        if (rb != null)
        {
            if (rb.mass >= minWeightToPress)
            {
                door.CloseDoor();
            }
        }
    }
}
