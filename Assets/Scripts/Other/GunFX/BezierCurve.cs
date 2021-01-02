using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BezierCurve : MonoBehaviour
{
    public Transform point1;
    public Transform point2;
    public Transform point3;

    public int vertexCount = 12;

    public LineRenderer lineRenderer;
    void Start()
    {}

    void Update()
    {
        var pointList = new List<Vector3>();

        for (float ratio = 0.0f ; ratio <= 1; ratio += 1.0f / vertexCount)
        {
            var tangentLineVertex1 = Vector3.Lerp(point1.position, point2.position, ratio);
            var tangentLineVertex2 = Vector3.Lerp(point2.position, point3.position, ratio);

            var bezierPoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);

            pointList.Add(bezierPoint);
        }
        lineRenderer.positionCount = pointList.Count;
        lineRenderer.SetPositions(pointList.ToArray());
    }
    
}
