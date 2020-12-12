using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TractorBeamTool : ATool
{

    private TheBeam TheBeam;

    private bool windUp;
    private float shootForce = 10;

    // Start is called before the first frame update
    void Start()
    {
        TheBeam = GameObject.Find("TheBeam").GetComponent<TheBeam>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (windUp && (shootForce < 40))
        {
            shootForce = shootForce + 0.5f;
            TheBeam.setWindup(shootForce/20); // needed for pulling of object towards player if winded up
        }
    }

    public override void Shoot(Ray ray, bool isRelease = false, bool isRightClick = false)
    {

        //rmb pressed
        if (isRightClick && !isRelease)
        {
            TheBeam.turnOnBeam();
        }

        //rmb released
        if (isRightClick && isRelease)
        {
            TheBeam.turnOffBeam();
        }

        //lmb released
        if (!isRightClick && isRelease)
        {
            //shoot object in beam
            Vector3 shootDirection = ray.direction;
            TheBeam.shoot(shootForce, shootDirection);
            TheBeam.turnOffBeam();

            // reset windup
            windUp = false;
            shootForce = 10;
        }

        //lmb pressed
        if (!isRightClick && !isRelease)
        {
            windUp = true;
        }
    }

    public override void Reset(bool isRelease)
    {

    }

    // delta = -120, 0, 120 depending on scroll direction
    public override void Scroll(float delta) 
    {
        TheBeam.changeDisplacementIntensity(delta);
    }
}
