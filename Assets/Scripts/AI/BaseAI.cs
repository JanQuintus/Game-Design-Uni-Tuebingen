using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(GravityObject))]

public class BaseAI : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected CapsuleCollider mainCollider;

    [Header("Walking")]
    [SerializeField] private float walkSpeed = 4.5f;
    [SerializeField] private float moveDamping = 5f;
    [Header("Ground Check")]
    [SerializeField] private Vector3 gcOffset = new Vector3(0, .1f, 0);
    [SerializeField] private float gcDistance = .3f;
    [SerializeField] private float gcRadius = .3f;
    [SerializeField] private LayerMask gcLayerMask;
    [Header("Jumping")]
    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private float maxObstacleHeight = 2f;
    [Header("Target Follow")]
    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private float maxSlope = .4f;
    [SerializeField] private float stoppingDistance = .4f;
    [Header("Audio")]
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] private AudioClip footStepClip;
    [SerializeField] private AudioClip landClip;
    [SerializeField] private float footStepDistance = 0.5f;


    protected bool _isAlive = true;

    protected AITarget _target;
    protected bool _canMove = true;
    protected Vector3 _nextPosition;
    protected Vector3 _lookPosition;
    protected Vector3 _smoothMove;
    protected GravityObject _gravity;
    protected Rigidbody _rb;
    protected bool _isGrounded = false;
    protected float _gravityChangeMoveBlockCD = 0;
    protected bool _atTarget = false;

    private float _nextFoodStep;

    protected virtual void Awake()
    {
        _gravity = GetComponent<GravityObject>();
        _rb = GetComponent<Rigidbody>();
        _nextPosition = transform.position;

        _gravity.OnGravityChanged += () => _gravityChangeMoveBlockCD = 0.4f;
    }

    private void FixedUpdate()
    {
        if (!_isAlive)
            return;

        animator.SetFloat("Speed", _rb.velocity.magnitude);
        animator.SetBool("isGrounded", _isGrounded);

        bool wasGrounded = _isGrounded;
        IsGrounded();

        if(!wasGrounded && _isGrounded)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(landClip);
        }

        Vector3 localVelocity = transform.InverseTransformDirection(_rb.velocity);
        if (_isGrounded && localVelocity.y <= 0)
        {
            mainCollider.material.staticFriction = Mathf.Clamp01(-_rb.velocity.y * -_rb.velocity.y);
            mainCollider.material.frictionCombine = PhysicMaterialCombine.Average;
        }
        else
        {
            mainCollider.material.staticFriction = 0;
            mainCollider.material.frictionCombine = PhysicMaterialCombine.Minimum;
        }


        if (_atTarget)
        {
            mainCollider.material.frictionCombine = PhysicMaterialCombine.Maximum;
        }


        if (_target == null)
        {
            _atTarget = false;
            return;
        }

        if (_isGrounded)
        {
            if(Physics.Raycast(transform.position + transform.TransformDirection(gcOffset), -transform.up, out RaycastHit hit, gcDistance, obstacleLayerMask))
            {
                Vector3 lookPosOnSurface = Vector3.ProjectOnPlane(_nextPosition, hit.normal) + (hit.distance + 0.01f - transform.TransformDirection(gcOffset).y) * hit.normal;
                Vector3 rayDir = lookPosOnSurface - transform.position;

                if (!Physics.Raycast(transform.position + transform.TransformDirection(gcOffset) + transform.forward, -transform.up, gcDistance, obstacleLayerMask))
                {
                    _rb.AddForce(jumpForce * transform.up, ForceMode.VelocityChange);
                    _isGrounded = false;
                    animator.SetTrigger("Jump");
                }else if (Physics.CapsuleCast(
                    transform.position + (mainCollider.radius + maxSlope) * transform.up,
                    transform.position + ((mainCollider.height / 2f) - mainCollider.radius) * transform.up,
                    mainCollider.radius,
                    rayDir,
                    out RaycastHit obstacle,
                    2f,
                    obstacleLayerMask))
                {
                    if((obstacleLayerMask == (obstacleLayerMask | (1 << obstacle.transform.gameObject.layer))) &&
                        !Physics.Raycast(transform.position + (transform.up * maxObstacleHeight), transform.forward, 2f, obstacleLayerMask))
                    {
                        _rb.AddForce(jumpForce * transform.up, ForceMode.VelocityChange);
                        _isGrounded = false;
                        animator.SetTrigger("Jump");
                        audioSource.pitch = 1;
                    }
                }
            }
        }

        if (Vector3.Distance(transform.position, _target.transform.position) >= stoppingDistance)
        {
            _atTarget = false;
            if (!_canMove)
                return;
            FindNextPosition();
            if (_isGrounded)
            {
                localVelocity = transform.InverseTransformDirection(_rb.velocity);
                Vector3 upVelocity = transform.TransformDirection(new Vector3(0, localVelocity.y, 0));
                Vector3 moveDir = transform.InverseTransformDirection((_nextPosition - transform.position));
                moveDir.y = 0;
                moveDir = moveDir.normalized;
                _smoothMove = Vector3.Lerp(_smoothMove, moveDir, Time.fixedDeltaTime * moveDamping);
                float speed = walkSpeed * 100f;
                if (_gravityChangeMoveBlockCD > 0)
                {
                    _gravityChangeMoveBlockCD -= Time.deltaTime;
                    _smoothMove = Vector2.zero;
                }
                Vector3 move = transform.TransformDirection(new Vector3(_smoothMove.x, 0, _smoothMove.z) * Time.fixedDeltaTime * speed) + upVelocity;
                _rb.velocity = move;

                _nextFoodStep -= _smoothMove.magnitude * speed;
                if (_nextFoodStep <= 0)
                {
                    _nextFoodStep = footStepDistance;
                    audioSource.pitch = Random.Range(0.9f, 1.1f);
                    if (Physics.Raycast(transform.position + transform.up, -transform.up, out RaycastHit hit, 2f))
                    {
                        if (hit.collider.GetComponent<CollisionSound>() != null)
                            audioSource.PlayOneShot(hit.collider.GetComponent<CollisionSound>().GetCollisionClip(), Random.Range(0.04f, 0.05f));
                    }

                    audioSource.PlayOneShot(footStepClip, Random.Range(0.5f, 1f));
                }
            }
        }
        else
            _atTarget = true;
    }

    private void LateUpdate()
    {
        if (!_isAlive)
            return;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up, -_gravity.GetLocalGravity().normalized) * transform.rotation, 4f * Time.deltaTime);

        if (!_isGrounded)
            _lookPosition = transform.position + transform.forward;

        if (_target == null)
            return;
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
            if (transform.InverseTransformPoint(wp).y > maxObstacleHeight)
                continue;
            if (!Physics.CapsuleCast(
                transform.position + (mainCollider.radius + 0.01f) * transform.up,
                transform.position + (mainCollider.height - mainCollider.radius - maxSlope) * transform.up,
                mainCollider.radius,
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

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position + (maxSlope) * transform.up + transform.forward, mainCollider.radius);
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position + ((mainCollider.height / 2f) - mainCollider.radius) * transform.up + transform.forward, mainCollider.radius);
        
    }
}
