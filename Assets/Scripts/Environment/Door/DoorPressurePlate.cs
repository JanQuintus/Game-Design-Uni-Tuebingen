using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPressurePlate : MonoBehaviour
{
    [SerializeField] private Door door;
    [SerializeField] private int minWeightToPress = 50;
    [SerializeField] private bool impactPressure = false;
    
    [SerializeField] private float impact_openDuration = 10.0f;

    private float _currentTime = 0.0f;

    private int _shader_Mass = Shader.PropertyToID("objmass");
    private bool _toldDoorToOpen = false;

    private void Awake()
    {
        gameObject.GetComponent<Renderer>().material.SetFloat(_shader_Mass, minWeightToPress);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_toldDoorToOpen) return;
        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb == null) //test if player walked over the pressure plate
        {
            rb = other.GetComponentInParent<Rigidbody>();
        }

        if (rb != null) 
        {
            if (rb.mass >= minWeightToPress)
            {
                _currentTime = Time.fixedTime;
                door.OpenDoor();
                _toldDoorToOpen = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb == null) //test if player walked over the pressure plate
        {
            rb = other.GetComponentInParent<Rigidbody>();
        }

        if (rb != null)
        {
            if (!_toldDoorToOpen && rb.mass >= minWeightToPress)
            {
                _currentTime = Time.fixedTime;
                door.OpenDoor();
                _toldDoorToOpen = true;
            }else if(_toldDoorToOpen && rb.mass < minWeightToPress)
            {
                door.CloseDoor();
                _toldDoorToOpen = false;
            }
        }
    }

    private void Update()
    {
        if (impactPressure)
        {
            if (_currentTime != 0 && (Time.fixedTime - _currentTime) > impact_openDuration)
            {
                door.CloseDoor();
                _currentTime = 0;
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (!_toldDoorToOpen) return;
        //original
        if (!impactPressure) {
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
                    _toldDoorToOpen = false;
                }
            }
        }
    }
}
