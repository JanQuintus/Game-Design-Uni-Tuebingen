using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Door : MonoBehaviour
{
    public bool IsOpen = false;
    public abstract void OpenDoor();
    public abstract void CloseDoor();
}
