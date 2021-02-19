using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Up_Down : MonoBehaviour
{
    private Vector3 Roof = new Vector3(-0.4f, 9.5f, -0.36f);
    private Vector3 Floor = new Vector3(0.4f, 0, -0.36f);
    public float speed;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.useGravity)
        {
            if (rb.transform.position.y >= Roof.y - 10)
            {

                rb.transform.position = rb.transform.position;
                //rb.useGravity = false;
                //Debug.Log(rb.transform.position.y);
            }
            else
            {
                rb.MovePosition(transform.position + (transform.up * speed * Time.deltaTime));
                //Debug.Log(rb.transform.position.y);
            }
                
        }
        else
        {
            if (rb.transform.position.y + 7 <= Floor.y + 1)
            {
                rb.transform.position = rb.transform.position;
                //rb.useGravity = true;
                //Debug.Log(rb.transform.position.y);
            }
            else
            {
                rb.MovePosition(transform.position + (-transform.up * speed * Time.deltaTime));
                //Debug.Log(rb.transform.position.y);
            }
        }
    }
}
