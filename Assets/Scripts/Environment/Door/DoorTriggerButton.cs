using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTriggerButton : MonoBehaviour
{
    [SerializeField] private DoorAnimated door;
    public static float openDuration = 5.0f;
    private float currentTime = 0.0f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {

            currentTime = Time.fixedTime;
            door.OpenDoor();


        }

        if (currentTime!=0 && (Time.fixedTime - currentTime) > openDuration)
        {
            door.CloseDoor();
        }
    }
}
