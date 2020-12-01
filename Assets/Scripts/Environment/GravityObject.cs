using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityObject : MonoBehaviour
{
    [SerializeField] private Vector3 localGravity = new Vector3(0, -9.81f, 0); //initialization with standard gravity
    private Rigidbody rb;

    void Awake() => rb = GetComponent<Rigidbody>();

    void FixedUpdate()
    {
        if (rb.useGravity)
            rb.AddForce(localGravity, ForceMode.Acceleration);
    }

    public Vector3 GetLocalGravity() => localGravity;
    public void SetLocalGravity(Vector3 newGravityVector) => localGravity = newGravityVector;
}
