using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tractorbeam_Activator : MonoBehaviour
{
    [SerializeField] private TractorBeamTool tractorbeam;
    [SerializeField] private int onTime = 0;
    [SerializeField] private int offTime = 50;
    [SerializeField] private int resetTime = 100;

    [SerializeField] private int _currentTime = 0;

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

        if (tractorbeam.IsActive() && tractorbeam.IsOccupied())
            tractorbeam.Scroll(-1f);
    }

    private void turnOnBeam()
    {
        tractorbeam.Shoot(new Ray(tractorbeam.transform.position, new Vector3(0, 0, 0)), false, true);
    }

    private void turnOffBeam()
    {
        tractorbeam.Shoot(new Ray(tractorbeam.transform.position, new Vector3(0, 0, 0)), true, true);
    }
}
