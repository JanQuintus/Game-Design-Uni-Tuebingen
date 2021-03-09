using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HangerDoorLever : MonoBehaviour
{
    [SerializeField] private GravityObject mutantGO;
    [SerializeField] private Transform lever;
    [SerializeField] private Transform hangerDoor;
    [SerializeField] private AudioClip openHangerDoorClip;

    public bool HangerDoorOpen { get; private set; }

    private void Update()
    {
        if (HangerDoorOpen) return;

        if (!mutantGO.GetRB().useGravity)
        {
            HangerDoorOpen = true;
            hangerDoor.DOMove(hangerDoor.transform.position + new Vector3(0, 10, 0), 10f);
            lever.DORotate(lever.rotation.eulerAngles + new Vector3(-30, 0, 0), .5f);
            SoundController.Instance.PlaySoundAtLocation(openHangerDoorClip, hangerDoor.position);
        }
    }

}
