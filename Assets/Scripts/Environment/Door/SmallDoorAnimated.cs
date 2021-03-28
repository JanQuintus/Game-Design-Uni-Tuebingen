using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SmallDoorAnimated : Door
{
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject startPos;
    [SerializeField] private GameObject endPos;

    [SerializeField] private int duration = 1;


    public override void OpenDoor()
    {
        door.transform.DOMove(endPos.transform.position, duration);
        IsOpen = true;
    }

    public override void CloseDoor()
    {
        door.transform.DOMove(startPos.transform.position, duration);
        IsOpen = false;
    }
}
