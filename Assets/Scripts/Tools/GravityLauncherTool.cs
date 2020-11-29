using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityLauncherTool : ATool
{
    GameObject projectile;
    private void Awake()
    {
        projectile = Resources.Load("projectile") as GameObject;
    }
    public override void Shoot(bool isRightClick = false)
    {

        // Check if a projectile is already active, if so destroy it. (Can be done on right and left click)
        GameObject[] activeBullet = GameObject.FindGameObjectsWithTag("GravityBomb");
        if (activeBullet.Length > 0)
        {
            Debug.Log("Bullets found");
            foreach(GameObject bullet in activeBullet) {
                Destroy(bullet);
            }
        }

        if (isRightClick)
        {
            GameObject bullet = Instantiate(projectile) as GameObject;
            bullet.tag = "GravityBomb";
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
