using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody), typeof(GravityObject))]
public class PlayerController : MonoBehaviour, PlayerInputActions.IPlayerActions
{
    public static PlayerController Instance;

    [SerializeField] private CapsuleCollider col;
    [SerializeField] private float height = 1.8f;
    [SerializeField] private WeaponBelt weaponBelt;

    [Header("Movement")]
    [SerializeField] private float moveDamping = 5f;
    [SerializeField] private float walkSpeed = 200f;
    [SerializeField] private float sprintSpeedMult = 1.75f;
    [SerializeField] private float crouchSpeedMult = .5f;

    [Header("Head")]
    [SerializeField] private Transform head;
    [SerializeField] private float mouseSensitivity = 40f;
    [SerializeField] private float maxPitch = 90;
    [SerializeField] private float minPitch = -70;
    [SerializeField] private float walkBobbingSpeed = 14f;
    [SerializeField] private Vector2 walkBobbingAmount = new Vector2(0.025f, 0.05f);

    [Header("Jump")]
    [SerializeField] private Vector3 jumpVelocity = new Vector3(0, 400, 0);
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Tool")]
    [SerializeField] private Transform toolHolder;
    [SerializeField] private ATool currentTool;
    [SerializeField] private float toolBobbingSpeed = 14f;
    [SerializeField] private Vector2 toolBobbingAmount = new Vector2(0.025f, 0.05f);

    [Header("Ground Check")]
    [SerializeField] private Vector3 gcOffset = new Vector3(0, .1f, 0);
    [SerializeField] private float gcDistance = .3f;
    [SerializeField] private float gcRadius = .3f;
    [SerializeField] private LayerMask gcLayerMask;

    [Header("Crouch")]
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private Vector3 ccOffset = new Vector3(0, -.1f, 0);
    [SerializeField] private float ccDistance = .3f;
    [SerializeField] private float ccRadius = .3f;
    [SerializeField] private LayerMask ccLayerMask;

    [Header("Interact")]
    [SerializeField] private float interactDistance = 2;
    [SerializeField] private LayerMask interactLayerMask;

    private PlayerInputActions _inputActions;
    private Rigidbody _rb;
    private GravityObject _gravity;
    private AITarget _target;

    private bool _isGrounded = false;
    private bool _isCrouching = false;
    private Vector2 _smoothMove = Vector2.zero;

    // From Input
    private Vector2 _move = Vector2.zero;
    private Vector2 _rotate = Vector2.zero;
    private bool _sprint = false;
    private bool _jumpRequest = false;
    private bool _jump = false;
    private bool _crouch = false;

    // Head
    private float _pitch = 0;
    private float _hbDefaultPosY = 0;
    private float _hbDefaultPosX = 0;
    private float _hbTimer = 0;
    private float _tbInitialDefaultPosY = 0;
    private float _tbDefaultPosY = 0;
    private float _tbDefaultPosX = 0;
    private float _tbDefaultPosZ = 0;
    private float _tbTimer = 0;

    #region Unity Functions

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _rb = GetComponent<Rigidbody>();
        _gravity = GetComponent<GravityObject>();
        _target = GetComponent<AITarget>();
        _inputActions = new PlayerInputActions();
        _inputActions.Player.SetCallbacks(this);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        col.height = height;
        col.center = new Vector3(col.center.x, height / 2f, col.center.z);
        Physics.IgnoreCollision(col, head.GetComponent<Collider>());
        _hbDefaultPosY = height - .2f;
        _hbDefaultPosX = head.localPosition.x;
        _tbDefaultPosY = toolHolder.localPosition.y;
        _tbInitialDefaultPosY = toolHolder.localPosition.y;
        _tbDefaultPosX = toolHolder.localPosition.x;
        _tbDefaultPosZ = toolHolder.localPosition.z;
    }

    private void FixedUpdate()
    {
        bool wasGrounded = _isGrounded;
        CheckIsGrounded();

        bool wasCrouching = _isCrouching;
        if (_crouch || (wasCrouching && KeepCrouching()))
            _isCrouching = true;
        else
            _isCrouching = false;
            
        if (_isCrouching)
        {
            col.height = crouchHeight;
            col.center = new Vector3(col.center.x, crouchHeight / 2f, col.center.z);
            _hbDefaultPosY =  crouchHeight - 0.2f;
        }
        else
        {
            col.height = height;
            col.center = new Vector3(col.center.x, height / 2f, col.center.z);
            _hbDefaultPosY = height - 0.2f;
        }

        Vector3 localVelocity = transform.InverseTransformDirection(_rb.velocity);
        Vector3 upVelocity = transform.TransformDirection(new Vector3(0, localVelocity.y, 0));
        _smoothMove = Vector2.Lerp(_smoothMove, _move, Time.fixedDeltaTime * moveDamping);
        float speed = walkSpeed * (_isCrouching ? crouchSpeedMult : (_sprint ? sprintSpeedMult : 1));
        Vector3 move = transform.TransformDirection(new Vector3(_smoothMove.x, 0, _smoothMove.y) * Time.fixedDeltaTime * speed) + upVelocity;
        _rb.velocity = move;

        if (_jumpRequest)
        {
            _jumpRequest = false;
            if (_isGrounded && !KeepCrouching())
            {
                Vector3 jumpVel = transform.TransformDirection(jumpVelocity);
                _rb.AddForce(jumpVel, ForceMode.Impulse);
            }
        }

        localVelocity = transform.InverseTransformDirection(_rb.velocity);
        if (localVelocity.y <= 0)
        {
            _rb.velocity += _gravity.GetLocalGravity() * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }else if (localVelocity.y > 0 && !_jump)
        {
            _rb.velocity += _gravity.GetLocalGravity() * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }

        localVelocity = transform.InverseTransformDirection(_rb.velocity);
        if (_isGrounded && localVelocity.y <= 0)
        {
            col.material.staticFriction = Mathf.Clamp01(-_rb.velocity.y * -_rb.velocity.y);
            col.material.frictionCombine = PhysicMaterialCombine.Average;
        }
        else
        {
            col.material.staticFriction = 0;
            col.material.frictionCombine = PhysicMaterialCombine.Minimum;
        }

        if (_isGrounded && !wasGrounded)
        {
            _tbDefaultPosY = _tbInitialDefaultPosY;
            head.DOLocalMoveY(head.localPosition.y - 0.25f, 0.1f);
        }
        else if(!_isGrounded && wasGrounded)
        {
            _tbDefaultPosY = _tbInitialDefaultPosY - 0.15f;
        }
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.T))
            SetCurrentTool(currentTool);

        transform.Rotate(new Vector3(0, _rotate.x, 0) * mouseSensitivity);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.FromToRotation(transform.up, -_gravity.GetLocalGravity().normalized) * transform.rotation, 4f * Time.deltaTime);
        // Head
        _pitch += -_rotate.y * mouseSensitivity;
        _pitch = Mathf.Clamp(_pitch, -maxPitch, -minPitch);
        head.localEulerAngles = new Vector3(_pitch, 0, 0);

        Vector3 localVelocity = transform.InverseTransformDirection(_rb.velocity);
        if ((Mathf.Abs(localVelocity.x) > 0.1f || Mathf.Abs(localVelocity.z) > 0.1f) && _isGrounded)
        {
            _hbTimer += Time.deltaTime * walkBobbingSpeed *_rb.velocity.magnitude;
            head.localPosition = new Vector3(
                Mathf.Lerp(head.localPosition.x, _hbDefaultPosX + Mathf.Sin(_hbTimer / 2f) * walkBobbingAmount.x, 8f * Time.deltaTime),
                Mathf.Lerp(head.localPosition.y, _hbDefaultPosY + Mathf.Sin(_hbTimer) * walkBobbingAmount.y, 8f * Time.deltaTime),
                head.localPosition.z);
        }
        else
        {
            head.localPosition = new Vector3(Mathf.Lerp(head.localPosition.x, _hbDefaultPosX, Time.deltaTime * (walkBobbingSpeed / 2f)),
                Mathf.Lerp(head.localPosition.y, _hbDefaultPosY, Time.deltaTime * walkBobbingSpeed), head.localPosition.z);
        }

        if ((Mathf.Abs(localVelocity.x) > 0.1f || Mathf.Abs(localVelocity.z) > 0.1f) && _isGrounded)
        {
            _tbTimer += Time.deltaTime * toolBobbingSpeed * _rb.velocity.magnitude;
            toolHolder.localPosition = new Vector3(
                Mathf.Lerp(toolHolder.localPosition.x, _tbDefaultPosX + Mathf.Sin(_tbTimer / 2f) * toolBobbingAmount.x, 8f * Time.deltaTime),
                Mathf.Lerp(toolHolder.localPosition.y, _tbDefaultPosY + Mathf.Sin(_tbTimer) * toolBobbingAmount.y, 8f * Time.deltaTime),
                toolHolder.localPosition.z);
        }
        else
        {
            _tbTimer = 0;
            toolHolder.localPosition = new Vector3(Mathf.Lerp(toolHolder.localPosition.x, _tbDefaultPosX, Time.deltaTime * (toolBobbingSpeed / 2f)),
                Mathf.Lerp(toolHolder.localPosition.y, _tbDefaultPosY, Time.deltaTime * toolBobbingSpeed), toolHolder.localPosition.z);
        }
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();
        _sprint = false;
        _move = Vector2.zero;
        _smoothMove = Vector2.zero;
        _rotate = Vector2.zero;
        _jump = false;
        _jumpRequest = false;
        _crouch = false;
    }

    #endregion

    #region Public Methods
    public void SetCurrentTool(ATool tool)
    {
        if (currentTool != null && currentTool.GetType() == tool.GetType())
            return;

        currentTool?.OnUnequip();

        toolHolder.DOKill();
        toolHolder.DOLocalMove(new Vector3(_tbDefaultPosX + 0.5f, _tbDefaultPosY - 0.5f, _tbDefaultPosZ - 0.5f), 0.25f).OnComplete(() =>
        {
            if (currentTool != null) currentTool.transform.localScale = Vector3.zero;
            toolHolder.DOLocalMoveZ(_tbDefaultPosZ, 0.25f);
            currentTool = tool;
            currentTool?.OnEquip();
            currentTool.transform.localScale = Vector3.one;
        });
    }

    public ATool GetCurrentTool() => currentTool;

    public Vector3 GetHeadPosition() => head.position;

    public AITarget GetAITarget() => _target;
    #endregion

    private void CheckIsGrounded()
    {
        _isGrounded = Physics.SphereCast(new Ray(transform.position + transform.TransformDirection(gcOffset), -transform.up), gcRadius, gcDistance, gcLayerMask);
    }

    private bool KeepCrouching()
    {
        return Physics.SphereCast(new Ray(transform.position + transform.TransformDirection(new Vector3(0, crouchHeight, 0)) + transform.TransformDirection(ccOffset), transform.up), ccRadius, ccDistance, ccLayerMask);
    }

    #region Input

    public void OnMove(InputAction.CallbackContext context)
    {
        _move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _rotate = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context) => _sprint = !_sprint;

    public void OnJumpStart(InputAction.CallbackContext context) { if (context.performed) _jumpRequest = true; }
    public void OnJumpHold(InputAction.CallbackContext context) => _jump = !_jump;

    public void OnCrouch(InputAction.CallbackContext context) => _crouch = !_crouch;

    public void OnShoot(InputAction.CallbackContext context) { if(context.performed || context.canceled) currentTool?.Shoot(new Ray(head.position, head.forward), context.canceled); }
    public void OnRightClick(InputAction.CallbackContext context) { if (context.performed || context.canceled) currentTool?.Shoot(new Ray(head.position, head.forward), context.canceled, true); }
    public void OnMiddleClick(InputAction.CallbackContext context) { if (context.performed || context.canceled) currentTool?.Reset(context.canceled); }
    public void OnScroll(InputAction.CallbackContext context) { currentTool?.Scroll(context.ReadValue<Vector2>().y); }

    public void OnNum1(InputAction.CallbackContext context) { if (context.performed) weaponBelt?.SetWeapon(0); }
    public void OnNum2(InputAction.CallbackContext context) { if (context.performed) weaponBelt?.SetWeapon(1); }
    public void OnNum3(InputAction.CallbackContext context) { if (context.performed) weaponBelt?.SetWeapon(2); }
    public void OnNum4(InputAction.CallbackContext context) { if (context.performed) weaponBelt?.SetWeapon(3); }

    public void OnInteract(InputAction.CallbackContext context) { 
        if(Physics.Raycast(head.position, head.forward, out RaycastHit hit, interactDistance, interactLayerMask))
            if (context.performed || context.canceled)
                hit.collider.GetComponent<AInteractive>()?.Interact(context.canceled);
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(gcOffset), gcRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(gcOffset) + -transform.up * gcDistance, gcRadius);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(new Vector3(0, crouchHeight, 0)) + transform.TransformDirection(ccOffset), ccRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(new Vector3(0, crouchHeight, 0)) + transform.TransformDirection(ccOffset) + transform.up * ccDistance, ccRadius);
    }
}
