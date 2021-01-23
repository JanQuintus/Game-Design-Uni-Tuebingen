using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(GravityObject))]

public class BaseAI : MonoBehaviour
{
    [Header("Walking")]
    [SerializeField] private float walkSpeed = 4.5f;
    [SerializeField] private float moveDamping = 5f;
    [Header("Ground Check")]
    [SerializeField] private Vector3 gcOffset = new Vector3(0, .1f, 0);
    [SerializeField] private float gcDistance = .3f;
    [SerializeField] private float gcRadius = .3f;
    [SerializeField] private LayerMask gcLayerMask;
    [Header("Target Follow")]
    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private float maxSlope = .4f;
    [SerializeField] private float stoppingDistance = .4f;

    protected bool _isAlive = true;

    protected AITarget _target;
    protected Vector3 _nextPosition;
    protected Vector3 _lookPosition;
    protected Vector3 _smoothMove;
    protected GravityObject _gravity;
    protected Rigidbody _rb;
    protected CapsuleCollider _collider;
    protected bool _isGrounded = false;
    protected float _gravityChangeMoveBlockCD = 0;

    protected virtual void Awake()
    {
        _gravity = GetComponent<GravityObject>();
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _nextPosition = transform.position;

        _gravity.OnGravityChanged += () => _gravityChangeMoveBlockCD = 0.4f;
    }

    private void FixedUpdate()
    {
        if (!_isAlive)
            return;
        IsGrounded();
        Vector3 localVelocity = transform.InverseTransformDirection(_rb.velocity);
        if (_isGrounded && localVelocity.y <= 0)
        {
            _collider.material.staticFriction = Mathf.Clamp01(-_rb.velocity.y * -_rb.velocity.y);
            _collider.material.frictionCombine = PhysicMaterialCombine.Average;
        }
        else
        {
            _collider.material.staticFriction = 0;
            _collider.material.frictionCombine = PhysicMaterialCombine.Minimum;
        }

        if (_target == null)
            return;

        if (_isGrounded)
        {
            if(Physics.Raycast(transform.position + transform.TransformDirection(gcOffset), -transform.up, out RaycastHit hit, gcDistance, obstacleLayerMask))
            {
                Vector3 lookPosOnSurface = Vector3.ProjectOnPlane(_nextPosition, hit.normal) + (hit.distance + 0.01f - transform.TransformDirection(gcOffset).y) * hit.normal;
                Vector3 rayDir = lookPosOnSurface - transform.position;

                if (Physics.Raycast(transform.position + (0.01f * transform.up), rayDir, out RaycastHit obstacle, 3f))
                {
                    if(obstacleLayerMask == (obstacleLayerMask | (1 << obstacle.transform.gameObject.layer)))
                    {
                        _rb.AddForce(transform.up * _rb.mass * 6f, ForceMode.Impulse);
                        _isGrounded = false;
                    }
                }
            }
        }

        if (Vector3.Distance(transform.position, _target.transform.position) >= stoppingDistance)
        {
            FindNextPosition();
            if (_isGrounded)
            {
                localVelocity = transform.InverseTransformDirection(_rb.velocity);
                Vector3 upVelocity = transform.TransformDirection(new Vector3(0, localVelocity.y, 0));
                Vector3 moveDir = transform.InverseTransformDirection((_nextPosition - transform.position).normalized);
                _smoothMove = Vector3.Lerp(_smoothMove, moveDir, Time.fixedDeltaTime * moveDamping);
                float speed = walkSpeed * 100f;
                if (_gravityChangeMoveBlockCD > 0)
                {
                    _gravityChangeMoveBlockCD -= Time.deltaTime;
                    _smoothMove = Vector2.zero;
                }
                Vector3 move = transform.TransformDirection(new Vector3(_smoothMove.x, 0, _smoothMove.z) * Time.fixedDeltaTime * speed) + upVelocity;
                _rb.velocity = move;
            }
        }
    }

    private void LateUpdate()
    {
        if (!_isAlive)
            return;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up, -_gravity.GetLocalGravity().normalized) * transform.rotation, 4f * Time.deltaTime);

        if (!_isGrounded)
            _lookPosition = transform.position + transform.forward;

        Vector3 localTarget;
        if (Vector3.Distance(transform.position, _target.transform.position) >= stoppingDistance)
            localTarget = transform.InverseTransformPoint(_lookPosition);
        else
            localTarget = transform.InverseTransformPoint(_target.transform.position);

        localTarget.y = 0;
        Vector3 target = transform.TransformPoint(localTarget);
        transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.LookRotation(target - transform.position, transform.up), 16f * Time.deltaTime);
    }

    private void IsGrounded()
    {
        _isGrounded =  Physics.SphereCast(new Ray(transform.position + transform.TransformDirection(gcOffset), -transform.up), gcRadius, gcDistance, gcLayerMask);
    }

    public void SetTarget(AITarget target)
    {
        if(target == null)
        {
            if (_target != null)
                _target.Untrack();
            _target = null;
            return;
        }
        _target = target;
        target.Track();
    }

    protected void FindNextPosition()
    {
        Vector3 targetPoint = _nextPosition;
        Vector3[] waypoints = _target.GetWayPoints();
        bool foundWP = false;
        for (int i = waypoints.Length - 1; i > 0; i--)
        {
            Vector3 wp = waypoints[i];
            Vector3 localWP = transform.InverseTransformPoint(wp);
            if(localWP.y > maxSlope)
                continue;

            if (!Physics.CapsuleCast(
                transform.position + (_collider.radius + 0.01f) * transform.up,
                transform.position + (_collider.height - _collider.radius - maxSlope) * transform.up,
                _collider.radius,
                wp - transform.position,
                Vector3.Distance(transform.position, wp) - 1f,
                obstacleLayerMask))
            {
                if (i < waypoints.Length - 1)
                    _lookPosition = waypoints[i + 1];
                else
                    _lookPosition = _target.transform.position;
                targetPoint = wp;
                foundWP = true;
                break;
            }
        }

        if (!foundWP)
            _lookPosition = transform.position + transform.forward;

        Debug.DrawRay(transform.position + new Vector3(0, 1, 0), _lookPosition - transform.position, Color.blue);

        _nextPosition = targetPoint;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(gcOffset), gcRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(gcOffset) + -transform.up * gcDistance, gcRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_nextPosition, 1.2f);
    }
}
