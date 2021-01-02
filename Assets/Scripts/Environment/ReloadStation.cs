using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadStation : AInteractive
{
    public override void Interact(bool isRelease)
    {
        if (isRelease)
            return;
        PlayerController.Instance.GetCurrentTool()?.Reload();
    }
}
