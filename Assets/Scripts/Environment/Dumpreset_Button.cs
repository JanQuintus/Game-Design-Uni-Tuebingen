using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dumpreset_Button : AInteractive
{
    [SerializeField] GameObject resetTrigger;

    public override void Interact(bool isRelease)
    {
        resetTrigger.GetComponent<dumpreset>().Release();
    }

    public override string GetText()
    {
        return "Release trash";
    }
}
