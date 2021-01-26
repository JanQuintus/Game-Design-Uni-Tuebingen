using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SmallDoorAnimated : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject startPos;
    [SerializeField] private GameObject endPos;

    [SerializeField] private int duration = 1;


    public void OpenDoor()
    {
        door.transform.DOMove(endPos.transform.position, duration);
    }

    public void CloseDoor()
    {
        door.transform.DOMove(startPos.transform.position, duration);
    }
}
