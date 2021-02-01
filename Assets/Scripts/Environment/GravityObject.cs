using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityObject : MonoBehaviour
{
    public System.Action OnGravityChanged;

    [SerializeField] private Vector3 localGravity = new Vector3(0, -9.81f, 0); //initialization with standard gravity

    [Header("Assign if not on same object")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider mainCollider;
    [SerializeField] private Renderer mainRenderer;

    public bool goEnabled = true;

    void Awake() {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (mainCollider == null) mainCollider = GetComponent<Collider>();
        if (mainRenderer == null) mainRenderer = GetComponent<Renderer>();
    }

    void FixedUpdate()
    {
        if (rb.useGravity && goEnabled)
            rb.AddForce(localGravity, ForceMode.Acceleration);
    }

    public Vector3 GetLocalGravity() => localGravity;
    public void SetLocalGravity(Vector3 newGravityVector) {
        if (localGravity != newGravityVector)
            OnGravityChanged?.Invoke();
        localGravity = newGravityVector;
    }

    public Rigidbody GetRB() => rb;
    public Collider GetMainCollider() => mainCollider;
    public Renderer GetMainRenderer() => mainRenderer;
}
