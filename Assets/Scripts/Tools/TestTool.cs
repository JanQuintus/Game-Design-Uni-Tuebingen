using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTool : ATool
{
    public override void Shoot(bool isRightClick = false)
    {
        Debug.Log("Shoot " + isRightClick);
    }
}
