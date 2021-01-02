using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Rigidbody), typeof(GravityObject))]

public class BaseAI : MonoBehaviour
{
    [Header("Ground Check")]
    [SerializeField] private Vector3 gcOffset = new Vector3(0, .1f, 0);
    [SerializeField] private float gcDistance = .3f;
    [SerializeField] private float gcRadius = .3f;
    [SerializeField] private LayerMask gcLayerMask;

    protected bool isAlive = true;

    private NavMeshAgent _agent;
    private GravityObject _gravity;
    private Vector3 _lastGravity;
    private bool _isRotating = false;
    private Vector3 _nextPosition;
    private Rigidbody _rb;
    private float _rotateTime = 0;
    private bool _isGrounded = false;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _gravity = GetComponent<GravityObject>();
        _lastGravity = _gravity.GetLocalGravity();
        _rb = GetComponent<Rigidbody>();
        _agent.updateUpAxis = true;
        _agent.updateRotation = true;
        _nextPosition = transform.position;
    }

    private void FixedUpdate()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(_rb.velocity);
        Vector3 upVelocity = transform.TransformDirection(new Vector3(0, localVelocity.y, 0));
        _rb.velocity = upVelocity;
    }

    private void Update()
    {
        if (!isAlive)
        {
            if (_agent.enabled)
                _agent.enabled = false;
            return;
        }
        IsGrounded();
        if (_lastGravity != _gravity.GetLocalGravity() && !_isRotating)
        {
            _isRotating = true;
            _lastGravity = _gravity.GetLocalGravity();
            _rotateTime = 0;
        }
        _agent.enabled = !_isRotating && _isGrounded;

        if (_isRotating)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up, -_gravity.GetLocalGravity().normalized) * transform.rotation, _rotateTime);
            _rotateTime += Time.deltaTime;


            if (_rotateTime >= 0.99f)
            {
                _isRotating = false;
                _rotateTime = 0;
                transform.rotation = Quaternion.FromToRotation(transform.up, -_gravity.GetLocalGravity().normalized) * transform.rotation;
            }
        }

        if (_agent.enabled && _agent.destination != _nextPosition)
            _agent.SetDestination(_nextPosition);
    }

    private void IsGrounded()
    {
        _isGrounded =  Physics.SphereCast(new Ray(transform.position + transform.TransformDirection(gcOffset), -transform.up), gcRadius, gcDistance, gcLayerMask);
    }

    public void MoveTo(Vector3 pos)
    {
        _nextPosition = pos;
    }

    public void GravityChange()
    {
        if (_lastGravity != _gravity.GetLocalGravity() && !_isRotating)
        {
            _isRotating = true;
            _lastGravity = _gravity.GetLocalGravity();
            _rotateTime = 0;
        }
        _agent.enabled = !_isRotating && _isGrounded;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(gcOffset), gcRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(gcOffset) + -transform.up * gcDistance, gcRadius);
    }
}
