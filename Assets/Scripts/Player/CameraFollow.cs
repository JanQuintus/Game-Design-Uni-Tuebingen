using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position, 40f * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, 40f * Time.deltaTime);
    }
}
