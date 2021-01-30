using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TractorBeamTool : ATool
{
    [SerializeField] private TheBeam TheBeam;
    [SerializeField] private float range = 100f;
    [SerializeField] private float maximalShootForce = 20f;

    [SerializeField] private float maximalHeat = 300;
    [SerializeField] private float heatUpPerTick = 1;
    [SerializeField] private float coolDownPerTick = 2;

    private float _heat = 0;
    private bool _windUp;
    private float _shootForce = 1; // current shoot force

    void FixedUpdate()
    {
        // windup mechanics
        if (_windUp && (_shootForce <= maximalShootForce))
        {
            _shootForce = _shootForce + 0.5f;
            TheBeam.setWindup(_shootForce / 10); // needed for pulling of object towards player if winded up
        }

        // overheating mechanics
        if (TheBeam.IsActive())
        {
            _heat = _heat + heatUpPerTick;
            OnFillChanged?.Invoke();
        } else if (_heat > 0)
        {
            _heat = _heat - coolDownPerTick;
            OnFillChanged?.Invoke();
        }

        if (_heat >= maximalHeat)
            TheBeam.turnOffBeam();
    }

    public override void Shoot(Ray ray, bool isRelease = false, bool isRightClick = false)
    {

        //rmb pressed
        if (isRightClick && !isRelease)
            TheBeam.turnOnBeam(range);

        //rmb released
        if (isRightClick && isRelease)
            TheBeam.turnOffBeam();

        //lmb released
        if (!isRightClick && isRelease)
        {
            //shoot object in beam
            Vector3 shootDirection = ray.direction;
            TheBeam.shoot(_shootForce, shootDirection);
            TheBeam.turnOffBeam();

            // reset windup
            _windUp = false;
            _shootForce = 1;
        }

        //lmb pressed
        if (!isRightClick && !isRelease)
            _windUp = true;
    }

    public override void Scroll(float delta)
    {
        TheBeam.changeDisplacementIntensity(delta);
    }

    public override void Reload() {}

    public override void Reset(bool isRelease) {}

    public override void OnEquip() { }

    public override void OnUnequip() => TheBeam.turnOffBeam();

    public override float getFillPercentage()
    {
        return 1f - (_heat / maximalHeat);
    }
}
