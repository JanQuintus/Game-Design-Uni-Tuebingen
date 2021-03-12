using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPressurePlate_Shot : MonoBehaviour
{
    [SerializeField] private Door door;
    [SerializeField] private int minWeightToPress = 50;

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
    


}
