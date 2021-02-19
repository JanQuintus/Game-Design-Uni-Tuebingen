using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement_Left_Right : MonoBehaviour
{
    private Vector3 LeftWall = new Vector3(4.6f, 4.5f, -0.36f);
    private Vector3 RightWall = new Vector3(-5.3f, 4.5f, -0.36f);
    public float speed;
    private Rigidbody rb;
    private float rightWall = 38f;
    private float lefttWall = 32f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.MovePosition(transform.position + (-transform.right * speed * Time.deltaTime));
        
        if (transform.position.x <= -38f)
        {
            //rb.transform.position = rb.transform.position;
            rb.MovePosition(transform.position + (transform.right * speed * Time.deltaTime));
            Debug.Log("Hitted Right Wall");
        }
        else if(transform.position.x >= -32f)
        {
            rb.transform.position = rb.transform.position;
            rb.MovePosition(transform.position + (-transform.right * speed * Time.deltaTime));
            Debug.Log("Hitted Left Wall");
        }
        
        Debug.Log(transform.position.x);
        
    }
}
