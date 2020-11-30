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
    public override void Shoot(bool isRightClick = false)
    {

        // Check if a projectile is already active, if so destroy it. (Can be done on right and left click)
       
        

        if (isRightClick)
        {
            if (newBullet)
            {
                if (!newBullet.GetComponent<GravityLauncherProjectile>().getActive())
                {
                    Destroy(newBullet);
                }
            }
            newBullet = Instantiate(projectile) as GameObject;
            newBullet.name = "GravityBomb";
            newBullet.transform.position = transform.position + Camera.main.transform.forward * 2;
            Rigidbody rb = newBullet.GetComponent<Rigidbody>();
            rb.velocity = Camera.main.transform.forward * 40;

           
        }
        else
        {
            destroyBullet();
        }
    }

    // Start is called before the first frame update
    void Start()
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
        {
            Destroy(bullet);
        }
    }

    
}
