using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dumpreset_Button : AInteractive
{
    [SerializeField] GameObject resetTrigger;

    public override void Interact(bool isRelease)
    {
        base.Interact(isRelease);
        resetTrigger.GetComponent<dumpreset>().Release();
        OnInteractionEnd.Invoke();
    }

    public override string GetText()
    {
        return "Release trash";
    }
}
