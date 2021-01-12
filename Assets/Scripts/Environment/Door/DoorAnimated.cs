using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DoorAnimated : MonoBehaviour
{
    [SerializeField] private GameObject doorLeft;
    [SerializeField] private GameObject doorRight;
    [SerializeField] private GameObject endPosLeft;
    [SerializeField] private GameObject endPosRight;
    [SerializeField] private GameObject startPosLeft;
    [SerializeField] private GameObject startPosRight;

    [SerializeField] private int duration = 1;



    private void Awake()
    {

    }

    public void OpenDoor()
    {
        doorLeft.transform.DOMove(endPosLeft.transform.position, duration);
        doorRight.transform.DOMove(endPosRight.transform.position, duration);
    }

    public void CloseDoor()
    {
        doorLeft.transform.DOMove(startPosLeft.transform.position, duration);
        doorRight.transform.DOMove(startPosRight.transform.position, duration);
    }
}
