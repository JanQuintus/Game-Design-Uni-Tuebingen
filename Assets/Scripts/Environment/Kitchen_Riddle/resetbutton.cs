using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resetbutton : AInteractive
{
    [SerializeField] Seal[] verschluesses;
    [SerializeField] Seal[] seals;

    [SerializeField] private static float openDuration = 5.0f;
    private float _currentTime = 0.0f;

    private void FixedUpdate()
    {
        if (_currentTime != 0 && (Time.fixedTime - _currentTime) > openDuration)
        {
            foreach (Seal verschluss in verschluesses)
            {
                verschluss.closeSeal(0f);
            }
            _currentTime = 0;
        }
    }

    public override void Interact(bool isRelease)
    {
        if (!isRelease)
        {
            foreach (Seal seal in seals)
            {
                seal.openSeal(0f);
            }
            foreach(Seal verschluss in verschluesses)
            {
                verschluss.openSeal(-90f);
            }
            _currentTime = Time.fixedTime;

        }
    }

    public override string GetText()
    {
        return "Reset";
    }
}
