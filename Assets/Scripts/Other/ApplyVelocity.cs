using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyVelocity : MonoBehaviour
{
    [SerializeField] Vector3 velocity;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _rb.velocity = velocity;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + velocity.normalized);
    }
}
