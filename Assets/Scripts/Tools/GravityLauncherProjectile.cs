using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityLauncherProjectile : MonoBehaviour
{
    // Start is called before the first frame update
    private bool isLocked = false;
    private bool active = false;
    private float liveTime;
    private Vector3 normal;
    public float radius = 20f;
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
            if (collision.gameObject.isStatic)
            {
                Debug.Log("locked");
                gameObject.GetComponent<Rigidbody>().isKinematic = true;
                isLocked = true;
                active = true;
                gameObject.GetComponent<SphereCollider>().radius = radius;
                gameObject.GetComponent<SphereCollider>().isTrigger = true;
                gameObject.GetComponent<SphereCollider>().center = Vector3.zero;
                initialGravityChanger(collision);

            }
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

    /**
     * initial gravity change after projectile is locked on the collision
     */
    private void initialGravityChanger(Collision collision)
    {
        normal = collision.GetContact(0).normal;
        Collider[] co = Physics.OverlapSphere(transform.position, radius);
        foreach(Collider collider in co)
        {

            double direction = Vector3.Dot(normal, (collider.transform.position - transform.position));
            if (direction < -2) changeGravity(collider, -normal * -9.81f);
            else changeGravity(collider, normal * -9.81f);

        }

    }

    private void OnTriggerEnter(Collider other)
    {

        double direction = Vector3.Dot(normal, (other.transform.position - transform.position));
        if (direction < -2) changeGravity(other, -normal * -9.81f);
        else changeGravity(other, normal * -9.81f);

    }

    private void OnTriggerExit(Collider other)
    {
        changeGravity(other, new Vector3 (0,-9.81f,0));
    }

    private void OnDestroy()
    {
        Collider[] co = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in co)
        {
            changeGravity(collider, new Vector3(0, -9.81f, 0));
        }
    }

    /**
     * change gravity of Collider collider to the Vector3 gravity
     */
    private void changeGravity(Collider collider, Vector3 gravity)
    {   
        /**
         * check if collider hast an gravityObject if not check if parent has one. 
         * Gravity of that GravityObject.
         */

        GravityObject go = collider.GetComponent<GravityObject>();
        if (!go) go = collider.GetComponentInParent<GravityObject>();
        if (go) go.SetLocalGravity(gravity);
    }

    private void OnTriggerStay(Collider other)
    {

        double direction = Vector3.Dot(normal, (other.transform.position - transform.position));
        if (direction < -2) changeGravity(other, -normal * -9.81f);
        else changeGravity(other, normal * -9.81f);
    }
}
