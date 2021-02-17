using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityLauncherFloor : AInteractive
{
    // Start is called before the first frame update
    [SerializeField] private List<GravityLauncherProjectile> _activeProjectiles;

    public override void Interact(bool isRelease)
    {
        if (isRelease) return;

        gameObject.SetActive(false);


        foreach( GravityLauncherProjectile proj in _activeProjectiles)
        {
            proj.SetDeactivateLifeTime(false);
        }
        OnInteractionEnd.Invoke();
    }

    public override string GetText()
    {
        return "Pick up Gravity Launcher";
    }
}
