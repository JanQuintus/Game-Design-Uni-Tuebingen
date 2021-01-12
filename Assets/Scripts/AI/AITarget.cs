using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITarget : MonoBehaviour
{
    [SerializeField] private float waypointDistance = 1f;
    [SerializeField] private int maxWayPoints = 50;

    private Queue<Vector3> _waypoints = new Queue<Vector3>();
    private Vector3 _lastPosition;
    private int tracking = 0;

    private void Awake()
    {
        _lastPosition = transform.position;
    }

    private void Update()
    {
        if (tracking == 0)
            return;
        if (transform.position == _lastPosition)
            return;

        if(Vector3.Distance(transform.position, _lastPosition) >= waypointDistance)
        {
            _waypoints.Enqueue(transform.position);
            _lastPosition = transform.position;
            if (_waypoints.Count > maxWayPoints)
                _waypoints.Dequeue();
        }    
    }

    public Vector3[] GetWayPoints() => _waypoints.ToArray();

    public Vector3 Track()
    {
        tracking++;
        return transform.position;
    }

    public void Untrack()
    {
        tracking--;
        if (tracking < 0)
            tracking = 0;
        if (tracking == 0)
            _waypoints.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        foreach(Vector3 wp in _waypoints)
        {
            Gizmos.DrawSphere(wp, .5f);
        }
    }
}
