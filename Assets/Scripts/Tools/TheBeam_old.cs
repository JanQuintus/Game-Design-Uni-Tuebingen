using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheBeam_old : MonoBehaviour
{

    public float pullForce = 1;
    public float minDist = 0.0f;

    private Collider objectInBeam;
    private float dist;

    private bool isOccupied = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //TODO: add offset to move object in beam via scrolling (obj.TransformDirection(vector);)

        if (dist <= minDist && isOccupied)
        {
            Rigidbody rb = objectInBeam.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.MovePosition(Vector3.Lerp(rb.position, transform.position, 40f * Time.deltaTime));
            rb.MoveRotation(Quaternion.Lerp(rb.rotation, transform.rotation, 40f * Time.deltaTime));
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

    private void OnTriggerStay(Collider other)
    {

        if (other == objectInBeam)
        {
            Vector3 targetPos = transform.position;
            Vector3 objectPos = objectInBeam.transform.position;

            float dx = objectPos.x - targetPos.x;
            float dy = objectPos.y - targetPos.y;
            float dz = objectPos.z - targetPos.z;

            dist = Mathf.Sqrt(dx * dx + dz * dz + dy * dy);

            if (dist > minDist)
            {
                //GravityObject objectInBeam = other.GetComponent<GravityObject>();
                //print(Beam.transform.position);
                //objectInBeam.transform.position = Beam.transform.position;


                //print(objectInBeam.name);
                //objectInBeam.AddForce(new Vector3(0, 50f, 0), ForceMode.Impulse);
                //objectInBeam.SetLocalGravity(Camera.main.transform.forward*-5);

                Vector3 forceDirection = targetPos - objectPos;
                Rigidbody rb = objectInBeam.GetComponent<Rigidbody>();
                rb.AddForce(forceDirection.normalized * pullForce, ForceMode.Impulse); // apply force towards center

            } /*else
            {
                Rigidbody rb = objectInBeam.GetComponent<Rigidbody>();
                rb.velocity = Vector3.zero;
                //objectInBeam.transform.parent = transform;
                objectInBeam.transform.position = Vector3.Lerp(transform.position, objectInBeam.transform.position, 40f * Time.deltaTime);
                objectInBeam.transform.rotation = Quaternion.Lerp(transform.rotation, objectInBeam.transform.rotation, 40f * Time.deltaTime);
            }*/
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (other == objectInBeam)
        {
            //objectInBeam.transform.position = transform.position;
            isOccupied = false;
            objectInBeam = null;

        }

    }

    public void turnOffBeam()
    {
        isOccupied = false;
        objectInBeam = null;
        gameObject.SetActive(false);
    }

    public void turnOnBeam()
    {
        gameObject.SetActive(true);
    }
}
