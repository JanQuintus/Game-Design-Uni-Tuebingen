using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BezierCurve : MonoBehaviour
{
    public Transform point1;
    public Transform point3;

    public int vertexCount = 12;

    public LineRenderer lineRenderer;
    void Start()
    {}

    void Update()
    {
        var pointList = new List<Vector3>();

        Vector3 p2 = point1.position + point1.forward * (Vector3.Distance(point1.position, point3.position) / 2f);

        for (float ratio = 0.0f ; ratio <= 1; ratio += 1.0f / vertexCount)
        {
            var tangentLineVertex1 = Vector3.Lerp(transform.InverseTransformPoint(point1.position), transform.InverseTransformPoint(p2), ratio);
            var tangentLineVertex2 = Vector3.Lerp(transform.InverseTransformPoint(p2), transform.InverseTransformPoint(point3.position), ratio);

            var bezierPoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);

            pointList.Add(bezierPoint);
        }
        lineRenderer.positionCount = pointList.Count;
        lineRenderer.SetPositions(pointList.ToArray());
    }
    
}
