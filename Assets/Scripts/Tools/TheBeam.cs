using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBeam : MonoBehaviour
{

    public float pullForce = 4;
    public float minDist = 5f;

    private Collider objectInBeam;
    private float dist;

    private bool isOccupied = false;

    private Vector3 displacement = new Vector3(0,0,0); // needed for displacement ob object inside beam due to scoll
    private int displacementIntensity = 0;

    private float maxDisplacement = 3; // adjust to beam length

    private float windUpStrength = 0; // this is the number which is decreased to visualize windup (stronger windup = object is closer to player)

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //TODO: add offset to move object in beam via scrolling (objectInBeam.TransformDirection(vector);)

        if (isOccupied)
        {
            Vector3 localPos = transform.InverseTransformDirection(transform.forward) * displacementIntensity;
            Vector3 lPos = transform.InverseTransformDirection(transform.forward * -1) * windUpStrength;
            Vector3 displace = transform.InverseTransformDirection(transform.forward * -1) * 9; // adjust floating to be closer to the player to allow more the beam to have more range
            Vector3 targetPos = transform.position + transform.TransformDirection(localPos) + transform.TransformDirection(lPos) + transform.TransformDirection(displace);

            Vector3 objectPos = objectInBeam.transform.position;

            float dx = objectPos.x - targetPos.x;
            float dy = objectPos.y - targetPos.y;
            float dz = objectPos.z - targetPos.z;

            dist = Mathf.Sqrt(dx * dx + dz * dz + dy * dy);

            Vector3 forceDirection = targetPos - objectPos;
            Rigidbody rb = objectInBeam.GetComponent<Rigidbody>();

            if (dist > minDist)
            {

                rb.AddForce(forceDirection.normalized * pullForce, ForceMode.Impulse); // apply force towards center

            }
            else
            {
                rb.velocity = rb.velocity * (1 - (5 * Time.deltaTime));
                rb.AddForce(forceDirection.normalized * pullForce, ForceMode.Impulse);

                //Vector3 lPos = transform.InverseTransformDirection(transform.forward * -1) * windUpStrength;
                //rb.AddForce(transform.TransformDirection(lPos), ForceMode.Impulse);
            }
        }

    }


    private void OnTriggerEnter(Collider other)
    {

        if ((!isOccupied) && (other.GetComponent("GravityObject") != null))
        {
            objectInBeam = other.GetComponent<Collider>();
            isOccupied = true;
        }


    }

    public void turnOffBeam()
    {
        isOccupied = false;
        objectInBeam = null;
        displacementIntensity = 0;
        windUpStrength = 0;
        gameObject.SetActive(false);
    }

    public void turnOnBeam()
    {
        gameObject.SetActive(true);
    }

    public void shoot(float shootForce, Vector3 forceDirection)
    {
        if (objectInBeam != null)
        {
            Rigidbody rb = objectInBeam.GetComponent<Rigidbody>();
            rb.AddForce(forceDirection.normalized * shootForce, ForceMode.Impulse);
        }
    }

    public void SetDisplacement(Vector3 newDir)
    {
        displacement = newDir;
    }

    public void changeDisplacementIntensity(float delta)
    {
        //move object in the distance
        if (displacementIntensity <= maxDisplacement)
        {
            if (delta > 0)
            {
                displacementIntensity = displacementIntensity + 1;
            }

        }

        //move object closer to the player
        if (displacementIntensity >= -maxDisplacement)
        {
            if (delta < 0)
            {
                displacementIntensity = displacementIntensity - 1;
            }
        }
    }

    public void setWindup(float value)
    {
        windUpStrength = value;
    }
}
