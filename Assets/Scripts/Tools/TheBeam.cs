using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBeam : MonoBehaviour
{

    [SerializeField] private Transform beamSource;
    [SerializeField] private float pullForce = 4;
    [SerializeField] private float minDist = 3f; // value which tells the distance to the gravitation point to start slowing down the object
    [SerializeField] private float maxDisplacementTo = 3; // maximal displacement due to scroll
    [SerializeField] private float maxDisplacementAway =10; // maximal displacement due to scroll
    [SerializeField] private float displacementCorrection = 9; // number which adjusts the position of the gravitation point. Needed to allow larger ranged beam
    [SerializeField] private float slowdown = 7; // this value determins the amount of slowdown if the object inside the beam is close to the gravitation point
    [SerializeField] private float pullForceInProximity = 2; // this value determines the pullforce when the object is close to the gravitation point
    [SerializeField] private float maxWeightLiftable = 10;
    [SerializeField] private Transform beamStart;
    [SerializeField] private BezierCurve laserEffect;
    [SerializeField] private GameObject sphereEffect;
    [SerializeField] private GameObject activeEffect;
    [SerializeField] private LayerMask layerMask;

    private class ObjectInBeam
    {
        public Collider col;
        public Rigidbody rb;
        public GravityObject go;

        public ObjectInBeam(Collider col, Rigidbody rb, GravityObject go)
        {
            this.col = col;
            this.rb = rb;
            this.go = go;
        }
    }

    private ObjectInBeam objectInBeam;
    private float dist; // distance of objectinbeam to center ie. gravitation point of beam
    private bool isOccupied = false;
    private int scrollDisplacement = 0; // current displacement due to scroll
    private float windUpStrength = 0; // this is the number which is decreased to visualize windup (stronger windup = object is closer to player)
    private int iter = 0;
    private bool _isActive = false;
    private float _distance = 100f;


    // Shaderchange VFX
    private int _activateEmssionID = Shader.PropertyToID("beamActive");

    // Gravity Paticle VFX
    private GameObject _inst_FX = null;
    private Material material;
    //////////////////////////////////////////////////////////////////////////////////////////////////////

    private void Awake()
    {
        _inst_FX = Instantiate(sphereEffect, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        _inst_FX.SetActive(false);
        activeEffect.SetActive(false);
    }

    void FixedUpdate()
    {
        if (!_isActive)
            return;

        if (!isOccupied)
        {
            _inst_FX.SetActive(false);
            if (Physics.Raycast(beamSource.position, beamSource.forward, out RaycastHit hit, _distance, layerMask))
            {
                Rigidbody rb = Utils.findRigidbody(hit.collider);
                if (rb == null)
                    return;

                GravityObject go = Utils.findGravityObject(hit.collider);
                if (go == null)
                    return;

                objectInBeam = new ObjectInBeam(hit.collider, rb, go);
                go.enabled = false;
                isOccupied = true;
            }
        }
        

        if (isOccupied)
        {
            // displace center of gravitation according to scroll and windup and correct for the length of the beam
            Vector3 localPos = transform.InverseTransformDirection(transform.forward) * scrollDisplacement;
            Vector3 lPos = transform.InverseTransformDirection(transform.forward * -1) * windUpStrength;
            Vector3 displace = transform.InverseTransformDirection(transform.forward * -1) * displacementCorrection; // adjust floating to be closer to the player to allow the beam to have more range
            Vector3 targetPos = transform.position + transform.TransformDirection(localPos) + transform.TransformDirection(lPos) + transform.TransformDirection(displace);

            Vector3 objectPos = objectInBeam.col.transform.position;
            Quaternion objectRot = objectInBeam.col.transform.rotation;

            laserEffect.point3 = objectInBeam.col.transform; //laser position
            
            // Sphere VFX And Shader FX
            _inst_FX.transform.SetPositionAndRotation(objectPos, objectRot);
            Vector3 rb_size = objectInBeam.col.bounds.size;
            _inst_FX.transform.localScale = new Vector3(rb_size.magnitude, rb_size.magnitude, rb_size.magnitude);
            _inst_FX.SetActive(true);
            
            material = objectInBeam.col.GetComponent<Renderer>().material;
            material.SetFloat(_activateEmssionID, 1);


            float dx = objectPos.x - targetPos.x;
            float dy = objectPos.y - targetPos.y;
            float dz = objectPos.z - targetPos.z;

            dist = Mathf.Sqrt(dx * dx + dz * dz + dy * dy);

            Vector3 forceDirection = targetPos - objectPos;
            Rigidbody rb = objectInBeam.rb;

            if ((rb.mass <= maxWeightLiftable) || !(rb.useGravity))
            {
                if (dist > minDist)
                {
                    iter = iter + 1;
                    if (iter >= 20)
                    {
                        rb.velocity = rb.velocity / 2;
                        iter = 0;
                    }

                    float factor = rb.mass / 5;
                    if (!rb.useGravity)
                    {
                        factor = rb.mass;
                    }
                    rb.AddForce(forceDirection.normalized * pullForce * Mathf.Max(1, factor), ForceMode.Impulse); // apply force towards center
                }
                else
                {
                    rb.velocity = rb.velocity * (1 - (slowdown * Time.deltaTime)); // slowdown when closer to the gravitation point
                    rb.AddForce(forceDirection.normalized * pullForceInProximity * rb.mass, ForceMode.Impulse);
                }
            }
        } 
    }

    public void turnOffBeam()
    {
        if (isOccupied && objectInBeam != null)
            objectInBeam.go.enabled = true;
        isOccupied = false;
        if (objectInBeam != null)
        {
            objectInBeam.go.enabled = true;
            objectInBeam.col.gameObject.GetComponent<Renderer>().material.SetFloat(_activateEmssionID, 0);
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
            Rigidbody rb = objectInBeam.rb;
            rb.velocity = Vector3.zero;
            rb.AddForce(forceDirection.normalized * Mathf.Max(1, rb.mass/2) * Mathf.Max(0, shootForce - (scrollDisplacement*2)), ForceMode.Impulse);
        }
    }

    // changes the displacement intensity due to scroll
    public void changeDisplacementIntensity(float delta)
    {
        //move object in the distance
        if (scrollDisplacement <= maxDisplacementAway)
        {
            if (delta > 0)
                scrollDisplacement = scrollDisplacement + 1;
        }

        //move object closer to the player
        if (scrollDisplacement >= -1 * maxDisplacementTo)
        {
            if (delta < 0)
                scrollDisplacement = scrollDisplacement - 1;
        }
    }

    public bool IsActive() => _isActive;

    public void setWindup(float value) =>  windUpStrength = value;
}
