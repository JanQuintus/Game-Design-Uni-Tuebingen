using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalGravityChangerTool : ATool
{
    [SerializeField] private GravityObject target;

    public override void Shoot(bool isRightClick = false)
    {
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit))
        {
            target?.SetLocalGravity(hit.normal * -9.81f);
        }
    }
}
