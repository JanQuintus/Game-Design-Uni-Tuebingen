using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBeam : MonoBehaviour
{

    public System.Action OnCatchObject;

    [SerializeField] private Transform beamSource;
    [SerializeField] private float pullForce = 4;
    [SerializeField] private float minDist = 3f; // value which tells the distance to the gravitation point to start slowing down the object
    [SerializeField] private float slowdown = 7; // this value determins the amount of slowdown if the object inside the beam is close to the gravitation point
    [SerializeField] private float pullForceInProximity = 2; // this value determines the pullforce when the object is close to the gravitation point
    [SerializeField] private float shootForceMult = .03f; // this value determines the pullforce when the object is close to the gravitation point
    [SerializeField] private float maxWeightLiftable = 10;
    [SerializeField] private float objMinDistOffset = 1f;
    [SerializeField] private Transform beamStart;
    [SerializeField] private BezierCurve laserEffect;
    [SerializeField] private GameObject sphereEffect;
    [SerializeField] private GameObject activeEffect;
    [SerializeField] private LayerMask layerMask;

    private GravityObject objectInBeam;
    private float dist; // distance of objectinbeam to center ie. gravitation point of beam
    private bool _isOccupied = false;
    private float scrollDisplacement = 0; // current displacement due to scroll
    private float windUpStrength = 0; // this is the number which is decreased to visualize windup (stronger windup = object is closer to player)
    private int iter = 0;
    private bool _isActive = false;
    private float _distance = 100f;
    private float _objMinDist = 0f;


    // Shaderchange VFX
    private int _activateEmssionID = Shader.PropertyToID("beamActive");

    // Gravity Paticle VFX
    private GameObject _inst_FX = null;
    //////////////////////////////////////////////////////////////////////////////////////////////////////

    private void Awake()
    {
        _inst_FX = Instantiate(sphereEffect, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        _inst_FX.transform.SetParent(transform);
        _inst_FX.SetActive(false);
        activeEffect.SetActive(false);
    }

    void FixedUpdate()
    {
        if (!_isActive)
            return;

        if (!_isOccupied)
        {
            _inst_FX.SetActive(false);
            if (Physics.Raycast(beamSource.position, beamSource.forward, out RaycastHit hit, _distance, layerMask))
            {
                GravityObject go = Utils.FindGravityObject(hit.collider);
                if (go == null)
                    return;

                objectInBeam = go;
                go.goEnabled = false;
                _isOccupied = true;
                scrollDisplacement = hit.distance / _distance;
                _objMinDist = Mathf.Max(Mathf.Max(hit.collider.bounds.size.x, hit.collider.bounds.size.y), hit.collider.bounds.size.z) + objMinDistOffset;
                OnCatchObject?.Invoke();
            }
        }
        

        if (_isOccupied)
        {
            // displace center of gravitation according to scroll and windup and correct for the length of the beam
            Vector3 localPos = Vector3.Lerp(transform.forward * (_objMinDist - (2f*windUpStrength)), transform.forward * _distance, scrollDisplacement);
            Vector3 targetPos = beamStart.position + localPos;
            Vector3 objectPos = objectInBeam.GetMainCollider().transform.position;

            Quaternion objectRot = objectInBeam.GetMainCollider().transform.rotation;
            Vector3 forceDirection = targetPos - objectPos;
            laserEffect.point3 = objectInBeam.GetMainCollider().transform; //laser position

            // Sphere VFX And Shader FX
            _inst_FX.transform.SetPositionAndRotation(objectPos, objectRot);
            Vector3 rb_size = objectInBeam.GetMainCollider().bounds.size;
            _inst_FX.transform.localScale = new Vector3(rb_size.magnitude, rb_size.magnitude, rb_size.magnitude);
            _inst_FX.SetActive(true);

            Utils.SetMaterialPropertyFloat(objectInBeam.GetMainRenderer(), _activateEmssionID, 1);

            dist = Vector3.Distance(objectPos, targetPos);
            Rigidbody rb = objectInBeam.GetRB();

            if (rb.mass <= maxWeightLiftable || !rb.useGravity)
            {
                if (dist > minDist)
                {
                    iter = iter + 1;
                    if (iter >= 20)
                    {
                        rb.velocity = rb.velocity / 2f;
                        iter = 0;
                    }

                    float factor = rb.mass / 5f;
                    if (!rb.useGravity)
                    {
                        factor = rb.mass;
                    }
                    rb.AddForce(forceDirection.normalized * pullForce * Mathf.Max(1, factor), ForceMode.Impulse); // apply force towards center
                }
                else
                {
                    rb.velocity = rb.velocity * (1f - (slowdown * Time.deltaTime)); // slowdown when closer to the gravitation point
                    rb.AddForce(forceDirection.normalized * pullForceInProximity * rb.mass * Mathf.Clamp01(dist), ForceMode.Impulse);
                }
            }

            if(dist > _distance + 2f)
            {
                _isOccupied = false;
                if (objectInBeam != null)
                {
                    objectInBeam.goEnabled = true;
                    Utils.SetMaterialPropertyFloat(objectInBeam.GetMainRenderer(), _activateEmssionID, 0);
                }

                objectInBeam = null;
                scrollDisplacement = 0;
                windUpStrength = 0;
                laserEffect.point3 = beamStart;
                // Shader
                _inst_FX?.SetActive(false);
            }
        } 
    }

    public void turnOffBeam()
    {
        _isOccupied = false;
        if (objectInBeam != null)
        {
            objectInBeam.goEnabled = true;
            Utils.SetMaterialPropertyFloat(objectInBeam.GetMainRenderer(), _activateEmssionID, 0);
        }

        objectInBeam = null;
        scrollDisplacement = 0;
        windUpStrength = 0;
        laserEffect.point3 = beamStart;
        // Shader
        _inst_FX?.SetActive(false);
        _isActive = false;
        activeEffect.SetActive(false);
    }

    public void turnOnBeam(float distance)
    {
        _isActive = true;
        _distance = distance;
        activeEffect.SetActive(true);
    }

    // This function should only be called along with turnOffBeam
    public void shoot(float shootForce, Vector3 forceDirection)
    {
        if (objectInBeam != null)
        {
            Rigidbody rb = objectInBeam.GetRB();
            if(rb.mass <= maxWeightLiftable || !rb.useGravity)
                rb.velocity = forceDirection.normalized * Mathf.Max(0, shootForce - (scrollDisplacement * 2f)) * shootForceMult / rb.mass;
        }
    }

    // changes the displacement intensity due to scroll
    public void changeDisplacementIntensity(float delta)
    {
        //move object in the distance
        scrollDisplacement = Mathf.Clamp01(scrollDisplacement + 0.025f * Mathf.Sign(delta));
    }

    public bool IsActive() => _isActive;
    public bool IsOccupied() => _isOccupied;

    public void setWindup(float value) =>  windUpStrength = value;
}
