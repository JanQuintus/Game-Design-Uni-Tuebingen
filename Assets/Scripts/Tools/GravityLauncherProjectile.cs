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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!active)
        {
            liveTime -= Time.deltaTime;
            Debug.Log(liveTime);
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
}
