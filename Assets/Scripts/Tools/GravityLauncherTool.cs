using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityLauncherTool : ATool
{
    GameObject projectile;
    GameObject bullet;
    private void Awake()
    {
        projectile = Resources.Load("projectile") as GameObject;
    }
    public override void Shoot(bool isRightClick = false)
    {

        // Check if a projectile is already active, if so destroy it. (Can be done on right and left click)
        if (bullet)
        {
            Destroy(bullet);
        }
        
        

        if (isRightClick)
        {
            bullet = Instantiate(projectile) as GameObject;
            bullet.name = "GravityBomb";
            bullet.transform.position = transform.position + Camera.main.transform.forward * 2;
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            rb.velocity = Camera.main.transform.forward * 40;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    
}
