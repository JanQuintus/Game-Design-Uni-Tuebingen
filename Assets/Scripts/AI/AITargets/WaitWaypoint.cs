using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitWaypoint : Waypoint
{
    [SerializeField] private float waitTime = 5f;

    private Dictionary<BaseAI, float> _waitTimes = new Dictionary<BaseAI, float>();

    private void Update()
    {
        try
        {
            foreach (BaseAI ai in _waitTimes.Keys)
                _waitTimes[ai] = _waitTimes[ai] - Time.deltaTime;
        }catch(InvalidOperationException e) { }
    }

    public override bool CanProceed(BaseAI ai)
    {
        if(_waitTimes.TryGetValue(ai, out float wt))
        {
            if(wt <= 0)
            {
                _waitTimes.Remove(ai);
                return true;
            }
        }
        else
            _waitTimes.Add(ai, waitTime);
        
        return false;
    }
}
