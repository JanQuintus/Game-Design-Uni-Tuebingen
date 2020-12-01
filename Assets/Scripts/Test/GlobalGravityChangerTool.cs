using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalGravityChangerTool : ATool
{
    public override void Shoot(bool isRightClick = false)
    {
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit))
        {
            Physics.gravity = hit.normal * -9.81f;
        }
    }
}
