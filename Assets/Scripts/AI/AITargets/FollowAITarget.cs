using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAITarget : AITarget
{
    [SerializeField] private float waypointDistance = 1f;
    [SerializeField] private int maxWayPoints = 50;

    protected Queue<Vector3> _waypoints = new Queue<Vector3>();
    private Vector3 _lastPosition;
    private int tracking = 0;

    public override void Track(BaseAI ai)
    {
        tracking++;
    }

    public override void Untrack(BaseAI ai)
    {
        tracking--;
        if (tracking < 0)
            tracking = 0;
        if (tracking == 0)
            _waypoints.Clear();
    }

    private void Awake()
    {
        _lastPosition = transform.position;
        _waypoints.Enqueue(transform.position);
    }

    private void Update()
    {
        if (tracking == 0)
            return;
        if (transform.position == _lastPosition)
            return;

        if (Vector3.Distance(transform.position, _lastPosition) >= waypointDistance)
        {
            _waypoints.Enqueue(transform.position);
            _lastPosition = transform.position;
            if (_waypoints.Count > maxWayPoints)
                _waypoints.Dequeue();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        foreach (Vector3 wp in _waypoints)
        {
            Gizmos.DrawSphere(wp, .5f);
        }
    }

    public override (Vector3, Vector3) GetNextWaypoint(BaseAI ai)
    {
        Vector3 targetPoint = ai.GetLastPosition();
        Vector3 lookPosition = ai.GetLastLookPosition();
        Vector3[] waypoints = _waypoints.ToArray();

        bool foundWP = false;
        for (int i = waypoints.Length - 1; i >= 0; i--)
        {
            Vector3 wp = waypoints[i];
            if (ai.transform.InverseTransformPoint(wp).y > ai.GetMaxObstacleHeight())
                continue;
            if (!Physics.CapsuleCast(
                ai.transform.position + (ai.GetMainCollider().radius + 0.01f) * ai.transform.up,
                ai.transform.position + (ai.GetMainCollider().height - ai.GetMainCollider().radius - ai.GetMaxSlope()) * ai.transform.up,
                ai.GetMainCollider().radius,
                wp - ai.transform.position,
                Vector3.Distance(ai.transform.position, wp) - 1f,
                ai.GetObstacleLayerMask()))
            {
                if (i < waypoints.Length - 1)
                    lookPosition = waypoints[i + 1];
                else
                    lookPosition = transform.position;
                targetPoint = wp;
                foundWP = true;
                break;
            }
        }

        if (!foundWP)
            lookPosition = transform.position + transform.forward;

        return (targetPoint, lookPosition);
    }
}
