using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class multi_pressure_plate_gym_door : Door
{
    [SerializeField] DoorPressurePlate[] doorPressurePlates;

    [SerializeField] private GameObject door;
    [SerializeField] private GameObject startPos;
    [SerializeField] private GameObject endPos;

    [SerializeField] private int duration = 1;

    private int numCorrectPressurePlates = 0;

    public override void OpenDoor()
    {
        numCorrectPressurePlates++;
        if (numCorrectPressurePlates == doorPressurePlates.Length)
        {
            door.transform.DOMove(endPos.transform.position, duration);
            IsOpen = true;
        }

    }

    public override void CloseDoor()
    {
        numCorrectPressurePlates--;
        door.transform.DOMove(startPos.transform.position, duration);
        IsOpen = false;
    }
}
