using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reload : MonoBehaviour
{
    [SerializeField]
    private Transform checkpoint;

    [SerializeField]
    private float distance;

    //Create referencies of Scripts later tog et method Reload()
    public GravityGunTool GGun;
    public GravityLauncherTool GLauncher;
    public MassExchangerTool MExchanger;
    public TractorBeamTool TBeam;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        distance = (checkpoint.transform.position - transform.position).magnitude;
        if (distance <= 0)
        {
            GGun.Reload();
            GLauncher.Reload();
            MExchanger.Reload();
            TBeam.Reload();
        }
    }
}
