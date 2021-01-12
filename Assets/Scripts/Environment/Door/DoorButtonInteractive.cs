using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButtonInteractive : AInteractive
{
    [SerializeField] private DoorAnimated door;
    [SerializeField] private static float openDuration = 5.0f;

    private float _currentTime = 0.0f;

    private void Update()
    {
        if (_currentTime!=0 && (Time.fixedTime - _currentTime) > openDuration)
        {
            door.CloseDoor();
            _currentTime = 0;
        }
    }

    public override void Interact(bool isRelease)
    {
        if (!isRelease && _currentTime == 0)
        {
            _currentTime = Time.fixedTime;
            door.OpenDoor();
        }
    }
}
