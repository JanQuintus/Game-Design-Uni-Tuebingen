using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class multi_pressure_plate_small_door : Door
{
    [SerializeField] DoorPressurePlate[] doorPressurePlates;

    [SerializeField] private GameObject door;
    [SerializeField] private GameObject startPos;
    [SerializeField] private GameObject endPos;

    [SerializeField] private int duration = 1;

    [SerializeField] private bool staysOpen = false;

    private int numCorrectPressurePlates = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    public override void OpenDoor()
    {
        numCorrectPressurePlates++;
        if (numCorrectPressurePlates == doorPressurePlates.Length)
        {
            door.transform.DOMove(endPos.transform.position, duration);
        }

    }

    public override void CloseDoor()
    {
        if (! staysOpen)
        {
            numCorrectPressurePlates--;
            door.transform.DOMove(startPos.transform.position, duration);
        }


    }
}
