using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityLauncherTool : ATool
{
    GameObject projectile;
    GameObject bullet;
    GameObject newBullet;
    private void Awake()
    {
        projectile = Resources.Load("projectile") as GameObject;
    }
    public override void Shoot(Ray ray, bool isRelease = false, bool isRightClick = false)
    {
        if (isRelease)
            return;
        // Check if a projectile is already active, if so destroy it. (Can be done on right and left click)

        if (!isRightClick)
        {
            if (newBullet)
            {
                if (!newBullet.GetComponent<GravityLauncherProjectile>().getActive())
                {
                    Destroy(newBullet);
                }
            }
            newBullet = Instantiate(projectile);
            newBullet.transform.right = ray.direction;
            newBullet.name = "GravityBomb";
            newBullet.transform.position = transform.position + Camera.main.transform.forward * 2;
            if (bullet)
            {
                newBullet.GetComponent<GravityObject>().SetLocalGravity(bullet.GetComponent<GravityObject>().GetLocalGravity());
            }
            
            Rigidbody rb = newBullet.GetComponent<Rigidbody>();
            rb.velocity = ray.direction * 40;
        }
        else
        {
            destroyBullet(); 
            if (newBullet)
                Destroy(newBullet);

            normalizeGravity();
        }
    }

    public override void Reset(bool isRelease)
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (newBullet)
        {
            if (newBullet.GetComponent<GravityLauncherProjectile>().getIsLocked())
            {
               
                destroyBullet();
                
                bullet = newBullet;
                bullet.GetComponent<GravityLauncherProjectile>().setIsLocked(false);
            }
        }
    }

    void destroyBullet()
    {
        if (bullet)
            Destroy(bullet);
    }

    void normalizeGravity()
    {
        GravityObject[] gravityObjects = GameObject.FindObjectsOfType<GravityObject>();

        foreach(GravityObject gravityObject in gravityObjects)
        {
            gravityObject.SetLocalGravity(new Vector3(0, -9.81f, 0));
        }
    }

    public override void Scroll(float delta){}
}
