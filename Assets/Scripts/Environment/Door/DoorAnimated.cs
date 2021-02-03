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

    [Header("Audio")]
    [SerializeField] private AudioClip doorOpenClip;
    [SerializeField] private AudioClip doorCloseClip;


    private void Awake()
    {

    }

    public void OpenDoor()
    {
        doorLeft.transform.DOMove(endPosLeft.transform.position, duration);
        doorRight.transform.DOMove(endPosRight.transform.position, duration);
        SoundController.Instance.PlaySoundAtLocation(doorOpenClip, transform.position);
    }

    public void CloseDoor()
    {
        doorLeft.transform.DOMove(startPosLeft.transform.position, duration);
        doorRight.transform.DOMove(startPosRight.transform.position, duration);
        SoundController.Instance.PlaySoundAtLocation(doorCloseClip, transform.position);
    }
}
