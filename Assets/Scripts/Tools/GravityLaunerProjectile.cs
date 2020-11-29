using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityLaunerProjectile : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Locked");
        if (collision.gameObject.isStatic)
        {
            gameObject.GetComponent<Rigidbody>().detectCollisions = false;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }
}
