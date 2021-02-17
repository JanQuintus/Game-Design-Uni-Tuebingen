using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GravityObject))]
public class PickupInteractive : AInteractive
{
    [SerializeField] private float waitOnPickup = 0.2f;
    [SerializeField] private float breakForce = 20f;
    [SerializeField] private float breakDistance = 1f;

    private GravityObject _go;
    private bool _pickedUp = false;
    private Transform _anchor = null;
    private float _followSpeed = 40f;

    private void Awake() => _go = GetComponent<GravityObject>();

    public override void Interact(bool isRelease)
    {
        base.Interact(isRelease);
        if (isRelease)
            return;
        OnInteractionEnd?.Invoke();
        if (canLift())
            PlayerController.Instance.PickUp(this);
    }

    public override string GetText()
    {
        if (!_pickedUp && canLift())
            return "Lift";
        else
            return "";
    }

    private void Update()
    {
        if(_pickedUp && _anchor)
        {
            transform.position = Vector3.Lerp(transform.position, _anchor.position, _followSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, _anchor.rotation, _followSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _anchor.position) > breakDistance)
                PlayerController.Instance.ReleasePickUp();
            if (!canLift())
                PlayerController.Instance.ReleasePickUp();
        }
    }

    private bool canLift()
    {
        return _go.GetRB().mass <= PlayerController.Instance.GetMaxPickupMass() || !_go.GetRB().useGravity;
    }

    public float GetBreakForce() => breakForce;
    public void Release(Vector3 velocity)
    {
        _go.GetRB().constraints = RigidbodyConstraints.None;
        _go.goEnabled = true;
        _go.GetRB().isKinematic = false;
        _go.GetRB().velocity = velocity;
        _pickedUp = false;
    }
    public GravityObject GetGravity() => _go;

    public IEnumerator PickUp(Transform anchor)
    {
        _anchor = anchor;
        _go.goEnabled = false;
        _go.GetRB().constraints = RigidbodyConstraints.FreezeRotation;
        yield return new WaitForSecondsRealtime(waitOnPickup);
        _pickedUp = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        _followSpeed = 10f;
        if (_pickedUp && collision.relativeVelocity.magnitude > breakForce)
            PlayerController.Instance.ReleasePickUp();
    }

    private void OnCollisionExit(Collision collision)
    {
        _followSpeed = 40f;
    }
}
