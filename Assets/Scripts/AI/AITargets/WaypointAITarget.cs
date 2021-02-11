using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaypointAITarget : AITarget
{
    [SerializeField] private Waypoint[] waypoints;
    [SerializeField] private float minWaypointDistance = 1f;
    [SerializeField] private bool visualize = false;

    private Dictionary<BaseAI, int> _aiWaypoints = new Dictionary<BaseAI, int>();
    private int _lastChildCount = 0;

    private void Update()
    {
        if(_lastChildCount <= transform.childCount)
            waypoints = transform.GetComponentsInChildren<Waypoint>();
    }

    public override void Track(BaseAI ai)
    {
        if (!_aiWaypoints.ContainsKey(ai))
            _aiWaypoints.Add(ai, FindClosestWaypointIndex(ai));
    }

    public override void Untrack(BaseAI ai)
    {
        if (_aiWaypoints.ContainsKey(ai))
            _aiWaypoints.Remove(ai);
    }

    public override (Vector3, Vector3) GetNextWaypoint(BaseAI ai)
    {
        Waypoint current = waypoints[_aiWaypoints[ai]];
        if (Vector3.Distance(ai.transform.position, current.transform.position) <= minWaypointDistance && current.CanProceed(ai))
            _aiWaypoints[ai] = (_aiWaypoints[ai] + 1) % waypoints.Length;
        return (waypoints[_aiWaypoints[ai]].transform.position, waypoints[_aiWaypoints[ai]].transform.position + waypoints[_aiWaypoints[ai]].transform.forward);
    }

    private int FindClosestWaypointIndex(BaseAI ai)
    {
        int closest = 0;
        float minDist = Mathf.Infinity;
        for(int i = 0; i < waypoints.Length; i++)
        {
            float dist = Vector3.SqrMagnitude(ai.transform.position - waypoints[i].transform.position);
            if(dist < minDist)
            {
                closest = i;
                minDist = dist;
            }
        }
        return closest;
    }

    private void OnDrawGizmos()
    {
        if (waypoints.Length == 0 || !visualize)
            return;
        Gizmos.color = Color.blue;
        for(int i = 0; i < waypoints.Length - 1; i++)
            Gizmos.DrawLine(waypoints[i].transform.position, waypoints[i + 1].transform.position);
        Gizmos.DrawLine(waypoints[waypoints.Length - 1].transform.position, waypoints[0].transform.position);

        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Length; i++)
            Gizmos.DrawSphere(waypoints[i].transform.position, .5f);
    }
}
