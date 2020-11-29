using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityObject : MonoBehaviour
{
    [SerializeField] private Vector3 localGravity = new Vector3(0, -9.81f, 0); //initialization with standard gravity
    private Rigidbody rb;
    [SerializeField] private bool useGravity = true;

    //private int counter = 0;

    void Awake()
    {
        gameObject.AddComponent<Rigidbody>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //counter = counter + 1;
        //if (counter == 100)
        //{
        //    SetLocalGravity(new Vector3(-9, 0, 0));
        //}
    }

    void FixedUpdate()
    {
        if (useGravity)
        {
            rb.AddForce(localGravity, ForceMode.Force); // continuous force with using mass of object
        }
    }

    public void SetLocalGravity(Vector3 newGravityVector)
    {
        localGravity = newGravityVector;
    }

    public void SetUseGravity(bool isGravity)
    {
        useGravity = isGravity;
    }
}
