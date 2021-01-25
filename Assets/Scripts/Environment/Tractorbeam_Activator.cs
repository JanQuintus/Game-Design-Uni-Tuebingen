using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tractorbeam_Activator : MonoBehaviour
{
    [SerializeField] private TractorBeamTool Tractorbeam;
    [SerializeField] private int onTime = 0;
    [SerializeField] private int offTime = 50;
    [SerializeField] private int resetTime = 100;

    private float _lastActivation = 0.0f;

    [SerializeField] private int _currentTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        //shoot tractorbeam, raycast is nonsense, but not needed since shooting is only possible when in hand
        turnOnBeam();
        turnOffBeam();

        //turn off tractorbeam, raycast is nonsense, but not needed since shooting is only possible when in hand
        //Tractorbeam.Shoot(new Ray(Tractorbeam.transform.position, new Vector3(0, 0, 0)), true, true);
    }


    void FixedUpdate()
    {
        if (_currentTime == onTime)
        {
            turnOnBeam();
        } else if (_currentTime == offTime)
        {
            turnOffBeam();
        } else if (_currentTime == resetTime)
        {
            _currentTime = -1;
        }

        _currentTime++;

    }

    private void turnOnBeam()
    {
        Tractorbeam.Shoot(new Ray(Tractorbeam.transform.position, new Vector3(0, 0, 0)), false, true);
    }

    private void turnOffBeam()
    {
        Tractorbeam.Shoot(new Ray(Tractorbeam.transform.position, new Vector3(0, 0, 0)), true, true);
    }
}
