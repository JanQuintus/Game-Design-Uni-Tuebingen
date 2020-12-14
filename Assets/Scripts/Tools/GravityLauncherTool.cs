using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityLauncherTool : ATool
{
    GameObject projectile;
    GameObject bullet;
    GameObject newBullet;
    public int ammo = 25;

    private void Awake()
    {
        projectile = Resources.Load("Projectile") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (newBullet)
        {
            if (newBullet.GetComponent<GravityLauncherProjectile>().getIsLocked())
            {
                if (bullet) Destroy(bullet);
                bullet = newBullet;
                bullet.GetComponent<GravityLauncherProjectile>().setIsLocked(false);
            }
        }
    }

    public override void Shoot(Ray ray, bool isRelease = false, bool isRightClick = false)
    {
        if (isRelease) return;
        // Check if a projectile is already active, if so destroy it. (Can be done on right and left click)

        if (!isRightClick && ammo > 0)
        {
            ammo--;
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
            
            Rigidbody rb = newBullet.GetComponent<Rigidbody>();
            rb.velocity = ray.direction * 40;
        }
        else
        {
            if (bullet) Destroy(bullet);
            if (newBullet) Destroy(newBullet);
        }
    }

   
    
    public override void Reset(bool isRelease) { }

    public override void Scroll(float delta){}
}
