using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TractorBeamTool : ATool
{
    [SerializeField] private float maxEnergy = 10;

    [SerializeField] private TheBeam TheBeam;

    private bool windUp;
    private float shootForce = 1; // current shoot force
    [SerializeField] private float maximalShootForce = 20f;
    [SerializeField] private float heat = 0;
    [SerializeField] private float maximalHeat = 300;
    [SerializeField] private float heatUpPerTick = 1;
    [SerializeField] private float coolDownPerTick = 2;

    private bool isBeamOn = false;


    void FixedUpdate()
    {
        // windup mechanics
        if (windUp && (shootForce <= maximalShootForce))
        {
            shootForce = shootForce + 0.5f;
            TheBeam.setWindup(shootForce/10); // needed for pulling of object towards player if winded up
        }

        // overheating mechanics
        if (isBeamOn)
        {
            heat = heat + heatUpPerTick;
        } else if(heat > 0)
        {
            heat = heat - coolDownPerTick;
        }

        if (heat >= maximalHeat)
        {
            turnOff();
        }
    }

    public override void Shoot(Ray ray, bool isRelease = false, bool isRightClick = false)
    {

        //rmb pressed
        if (isRightClick && !isRelease)
        {
            turnOn();
        }

        //rmb released
        if (isRightClick && isRelease)
        {
            turnOff();
        }

        //lmb released
        if (!isRightClick && isRelease)
        {
            //shoot object in beam
            Vector3 shootDirection = ray.direction;
            TheBeam.shoot(shootForce, shootDirection);
            turnOff();


            // reset windup
            windUp = false;
            shootForce = 1;
        }

        //lmb pressed
        if (!isRightClick && !isRelease)
        {
            windUp = true;
        }
    }

    private void turnOn()
    {
        TheBeam.turnOnBeam();
        isBeamOn = true;
    }

    private void turnOff()
    {
        TheBeam.turnOffBeam();
        isBeamOn = false;
    }

    public override void Scroll(float delta) 
    {
        TheBeam.changeDisplacementIntensity(delta);
    }

    public override void Reload() => shootForce = maxEnergy;

    public override void Reset(bool isRelease) {}

    public override void OnEquip()
    { }

    public override void OnUnequip()
    { }

}
