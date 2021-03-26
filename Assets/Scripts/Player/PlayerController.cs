using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody), typeof(GravityObject), typeof(Interactor))]
public class PlayerController : MonoBehaviour, PlayerInputActions.IPlayerActions
{
    public static PlayerController Instance;

    [SerializeField] private Camera mainCanera;
    [SerializeField] private Camera toolCamera;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private CapsuleCollider col;
    [SerializeField] private float height = 1.8f;
    [SerializeField] private ToolBelt toolBelt;
    [SerializeField] private SpriteRenderer damageScreenOverlay;
    [SerializeField] private float maxPickupMass = 15;

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

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip footStepClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip landClip;
    [SerializeField] private AudioClipBundle damageClipBundle;
    [SerializeField] private AudioClip crouchClip;
    [SerializeField] private AudioClip standUpClip;
    [SerializeField] private AudioSource heartBeatSource;
    [SerializeField] private float heartBeatNormalSpeed = 2f;
    [SerializeField] private AudioClip heartBeat1;
    [SerializeField] private AudioClip heartBeat2;
    [SerializeField] private float footStepDistance = 0.5f;

    private PlayerInputActions _inputActions;
    private Rigidbody _rb;
    private GravityObject _gravity;
    private AITarget _target;
    private Interactor _interactor;

    private bool _isGrounded = false;
    private bool _isCrouching = false;
    private bool _inputEnabled = true;
    private float _gravityChangeMoveBlockCD = 0;
    private Vector2 _smoothMove = Vector2.zero;
    private Health _health;
    private float _slopeSpeedMult = 1f;
    private int _inputBlocks = 0;

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

    private bool _heartBeat1Played = false;
    private float _heartBeatSpeed;
    private float _heartBeatTimer;
    private float _nextFoodStep;

    // PickUp
    private Transform _pickUpParent = null;
    private PickupInteractive _pickUpObject = null;

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
        _interactor = GetComponent<Interactor>();
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
        _health = GetComponentInParent<Health>();
        _nextFoodStep = footStepDistance;
        _heartBeatSpeed = heartBeatNormalSpeed;
        _heartBeatTimer = heartBeatNormalSpeed;
        _pickUpParent = Instantiate(new GameObject("PickUpParent"), head).transform;

        _health.OnDeath.AddListener(() =>
        {
            Debug.Log("You died!");
            _health.Revive();
        });

        _health.OnDamaged.AddListener((damage) =>
        {
            head.DOShakePosition(0.4f, 0.25f);
            toolHolder.DOShakePosition(0.2f, 0.1f);
            damageScreenOverlay?.material.SetFloat("_Strength", 1f - _health.GetHealthPercentage());
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(damageClipBundle.GetRandomClip(), Mathf.Clamp01(damage / 10f));
        });
        _health.OnHealed.AddListener((healed) =>
        {
            damageScreenOverlay?.material.SetFloat("_Strength", 1f - _health.GetHealthPercentage());
        });

        _gravity.OnGravityChanged += () => _gravityChangeMoveBlockCD = 0.4f;

        GameManager.OnPauseGame += () =>
        {
            BlockInput();
        };
        GameManager.OnUnpauseGame += () =>
        {
            UnblockInput();
        };
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

        if (!wasCrouching && _isCrouching)
        {
            audioSource.pitch = 1;
            audioSource.PlayOneShot(crouchClip);
        }
        if (wasCrouching && !_isCrouching)
        {
            audioSource.pitch = 1;
            audioSource.PlayOneShot(standUpClip);
        }
        

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
        float speed = walkSpeed * (_isCrouching ? crouchSpeedMult : (_sprint ? sprintSpeedMult : 1)) * (_isGrounded ? _slopeSpeedMult : 1f);
        if (_gravityChangeMoveBlockCD > 0)
        {
            _gravityChangeMoveBlockCD -= Time.deltaTime;
            _smoothMove = Vector2.zero;
        }
        Vector3 move = transform.TransformDirection(new Vector3(_smoothMove.x, 0, _smoothMove.y) * Time.fixedDeltaTime * speed) + upVelocity;
        _rb.velocity = move;
        if(_isGrounded) 
            _nextFoodStep -= _smoothMove.magnitude * speed;
        if(_nextFoodStep <= 0)
        {
            _nextFoodStep = footStepDistance;
            if (_isGrounded)
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                if (Physics.Raycast(transform.position + transform.up, -transform.up, out RaycastHit hit, 2f))
                {
                    if (hit.collider.GetComponent<CollisionSound>() != null)
                        audioSource.PlayOneShot(hit.collider.GetComponent<CollisionSound>().GetCollisionClip(), Random.Range(0.04f, 0.05f));
                }
                
                audioSource.PlayOneShot(footStepClip, Random.Range(0.5f, 1f));
            }
        }

        if (_jumpRequest)
        {
            _jumpRequest = false;
            if (_isGrounded && !KeepCrouching() && _slopeSpeedMult > 0)
            {
                Vector3 jumpVel = transform.TransformDirection(jumpVelocity);
                _rb.AddForce(jumpVel, ForceMode.Impulse);
                audioSource.pitch = 1;
                audioSource.PlayOneShot(jumpClip);
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
        if (_isGrounded && (localVelocity.y <= -0.5 || _smoothMove.sqrMagnitude < 0.2f))
        {
            col.material.staticFriction = 1;
            col.material.frictionCombine = PhysicMaterialCombine.Maximum;
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
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(landClip, Mathf.Clamp01(_rb.velocity.magnitude - 5f));
        }
        else if(!_isGrounded && wasGrounded)
        {
            _tbDefaultPosY = _tbInitialDefaultPosY - 0.15f;
        }
    }

    private void LateUpdate()
    {
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

    private void Update()
    {
        if (_pickUpParent)
        {
            _pickUpParent.transform.forward = transform.forward;
        }

        _heartBeatSpeed = heartBeatNormalSpeed * Mathf.Max(0.2f, _health.GetHealthPercentage());
        heartBeatSource.volume = (1f - _health.GetHealthPercentage()) * 2f + 0.1f;
        _heartBeatTimer -= Time.deltaTime;
        if(_heartBeatTimer / _heartBeatSpeed <= 0.3f && !_heartBeat1Played)
        {
            _heartBeat1Played = true;
            heartBeatSource.PlayOneShot(heartBeat1);
        }
        if (_heartBeatTimer / _heartBeatSpeed <= 0f)
        {
            _heartBeat1Played = false;
            _heartBeatTimer = _heartBeatSpeed;
            heartBeatSource.PlayOneShot(heartBeat2);
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
        if (tool != null && toolBelt.GetCurrentSlot() != null && toolBelt.GetCurrentSlot().Tool != null && toolBelt.GetCurrentSlot().Tool.GetType() == tool.GetType())
            return;

        if (toolBelt.GetCurrentSlot() != null && toolBelt.GetCurrentSlot().Tool != null)
            toolBelt.GetCurrentSlot().Tool?.OnUnequip();

        toolHolder.DOKill();
        toolHolder.DOLocalMove(new Vector3(_tbDefaultPosX + 0.5f, _tbDefaultPosY - 0.5f, _tbDefaultPosZ - 0.5f), 0.25f).OnComplete(() =>
        {
            foreach (ToolBelt.ToolSlot slot in toolBelt.GetToolSlots())
                slot.Tool.transform.localScale = Vector3.zero;

            toolHolder.DOLocalMoveZ(_tbDefaultPosZ, 0.25f);
            if (toolBelt.GetCurrentSlot() != null && toolBelt.GetCurrentSlot().Tool != null)
            {
                toolBelt.GetCurrentSlot().Tool?.OnEquip();
                toolBelt.GetCurrentSlot().Tool.transform.localScale = Vector3.one;
            }
        });
    }

    public ATool GetCurrentTool() => toolBelt.GetCurrentSlot().Tool;

    public Vector3 GetHeadPosition() => head.position;

    public AITarget GetAITarget() => _target;

    public ToolBelt GetToolBelt() => toolBelt;

    public Camera GetMainCamera() => mainCanera;

    public void BlockInput()
    {
        _inputEnabled = false;
        _inputActions.Disable();
        _move = Vector2.zero;
        _inputBlocks++;
    }

    public void UnblockInput()
    {
        _inputBlocks--;
        if (_inputBlocks < 0)
            _inputBlocks = 0;
        if (_inputBlocks == 0)
        {
            _inputEnabled = true;
            _inputActions.Enable();
        }
    }

    public float GetMaxPickupMass() => maxPickupMass;

    public void PickUp(PickupInteractive pickup)
    {
        if (_pickUpObject != null)
            return;
        _pickUpObject = pickup;
        float pickUpDistance = pickup.GetGravity().GetMainCollider().bounds.size.z + 1f;
        _pickUpParent.transform.position = head.position + head.forward * pickUpDistance;
        StartCoroutine(pickup.PickUp(_pickUpParent));
        toolBelt.SetEmptySlot();
    }

    public void ReleasePickUp()
    {
        if (_pickUpObject == null)
            return;
        _pickUpObject.Release(_rb.velocity);
        _pickUpObject = null;
        toolBelt.SetPreviousSlot();
    }
    #endregion

    private void CheckIsGrounded()
    {
        if (Physics.SphereCast(new Ray(transform.position + transform.TransformDirection(gcOffset), -transform.up), gcRadius, out RaycastHit hit, gcDistance, gcLayerMask))
        {
            if (transform.InverseTransformDirection(_rb.velocity).y > -.5f)
            {
                _slopeSpeedMult = Vector3.Dot(hit.normal, transform.up);
                if (_slopeSpeedMult <= 0.65)
                    _slopeSpeedMult = 0;
            }
            else
                _slopeSpeedMult = 1f;
                
            _isGrounded = true;
            return;
        }
        _isGrounded = false;
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

    public void OnSprint(InputAction.CallbackContext context) { if (context.performed || context.canceled) _sprint = context.performed; }
    
    public void OnJumpStart(InputAction.CallbackContext context) { if (context.performed) _jumpRequest = true; }
    public void OnJumpHold(InputAction.CallbackContext context) => _jump = !_jump;

    public void OnCrouch(InputAction.CallbackContext context) { if (context.performed || context.canceled) _crouch = context.performed; }

    public void OnShoot(InputAction.CallbackContext context) {
        if (toolBelt.GetCurrentSlot() == null || toolBelt.GetCurrentSlot().Tool == null)
            return;
        if (context.performed || context.canceled)
            toolBelt.GetCurrentSlot().Tool.Shoot(new Ray(head.position, head.forward), context.canceled);
     }
    public void OnRightClick(InputAction.CallbackContext context) {
        if (toolBelt.GetCurrentSlot() == null || toolBelt.GetCurrentSlot().Tool == null)
            return;
        if (context.performed || context.canceled) 
            toolBelt.GetCurrentSlot().Tool.Shoot(new Ray(head.position, head.forward), context.canceled, true); 
    }
    public void OnMiddleClick(InputAction.CallbackContext context) {
        if (toolBelt.GetCurrentSlot() == null || toolBelt.GetCurrentSlot().Tool == null)
            return;
        if (context.performed || context.canceled) 
            toolBelt.GetCurrentSlot().Tool.Reset(context.canceled); 
    }
    public void OnScroll(InputAction.CallbackContext context) {
        if (toolBelt.GetCurrentSlot() == null || toolBelt.GetCurrentSlot().Tool == null)
            return;
        toolBelt.GetCurrentSlot().Tool.Scroll(context.ReadValue<Vector2>().y); 
    }

    public void OnNum1(InputAction.CallbackContext context) { if (context.performed) toolBelt?.SetTool(1); }
    public void OnNum2(InputAction.CallbackContext context) { if (context.performed) toolBelt?.SetTool(2); }
    public void OnNum3(InputAction.CallbackContext context) { if (context.performed) toolBelt?.SetTool(3); }
    public void OnNum4(InputAction.CallbackContext context) { if (context.performed) toolBelt?.SetTool(4); }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed) GameManager.PauseGame();
    }

    public void OnInteract(InputAction.CallbackContext context) { 
        if (context.performed || context.canceled)
        {
            if (_pickUpObject != null && !context.canceled)
                ReleasePickUp();
            else
                _interactor.PerformInteract(context.canceled);
        }
    }

    public void OnToggleUIAndWeapon(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            toolCamera.enabled = !toolCamera.enabled;
            uiCamera.enabled = !uiCamera.enabled;
        }
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
