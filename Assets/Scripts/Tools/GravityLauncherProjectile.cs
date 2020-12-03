using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityLauncherProjectile : MonoBehaviour
{
    // Start is called before the first frame update
    private bool isLocked = false;
    private bool active = false;
    private float liveTime;
    private void Awake()
    {
        liveTime = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!active)
        {
            liveTime -= Time.deltaTime;
            if (liveTime < 0)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.isStatic)
        {
            Debug.Log("Locked");
            gameObject.GetComponent<Rigidbody>().detectCollisions = false;
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            isLocked = true;
            active = true;
            changeGravity(collision);
        }
    }
    
    public bool getIsLocked()
    {
        return isLocked;
    }

    public void setIsLocked(bool locked)
    {
        isLocked = locked;
    }

    public bool getActive()
    {
        return active;
    }

    private void changeGravity(Collision collision)
    {
        Vector3 normal = collision.GetContact(0).normal;
        GravityObject[] gravityObjects = GameObject.FindObjectsOfType<GravityObject>();
        foreach(GravityObject gravityObject in gravityObjects)
        {
            gravityObject.SetLocalGravity(normal * -9.81f);
        }
    }
}
