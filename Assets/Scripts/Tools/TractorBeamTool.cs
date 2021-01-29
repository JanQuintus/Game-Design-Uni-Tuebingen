using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TractorBeamTool : ATool
{
    [SerializeField] private TheBeam TheBeam;

    [SerializeField] private float maximalShootForce = 20f;

    [SerializeField] private float maximalHeat = 300;
    [SerializeField] private float heatUpPerTick = 1;
    [SerializeField] private float coolDownPerTick = 2;

    private bool _isBeamOn = false;
    private float _heat = 0;
    private bool _windUp;
    private float _shootForce = 1; // current shoot force

    private void Awake()
    {
        TheBeam.gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        // windup mechanics
        if (_windUp && (_shootForce <= maximalShootForce))
        {
            _shootForce = _shootForce + 0.5f;
            TheBeam.setWindup(_shootForce / 10); // needed for pulling of object towards player if winded up
        }

        // overheating mechanics
        if (_isBeamOn)
        {
            _heat = _heat + heatUpPerTick;
            OnFillChanged?.Invoke();
        } else if (_heat > 0)
        {
            _heat = _heat - coolDownPerTick;
            OnFillChanged?.Invoke();
        }

        if (_heat >= maximalHeat)
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
            TheBeam.shoot(_shootForce, shootDirection);
            turnOff();


            // reset windup
            _windUp = false;
            _shootForce = 1;
        }

        //lmb pressed
        if (!isRightClick && !isRelease)
        {
            _windUp = true;
        }
    }

    private void turnOn()
    {
        TheBeam.gameObject.SetActive(true);
        TheBeam.turnOnBeam();
        _isBeamOn = true;
    }

    private void turnOff()
    {
        TheBeam.turnOffBeam();
        TheBeam.gameObject.SetActive(false);
        _isBeamOn = false;
    }

    public override void Scroll(float delta)
    {
        TheBeam.changeDisplacementIntensity(delta);
    }

    public override void Reload() {}

    public override void Reset(bool isRelease) {}

    public override void OnEquip()
    { }

    public override void OnUnequip()
    {
        turnOff();
    }

    public override float getFillPercentage()
    {
        return 1f - (_heat / maximalHeat);
    }
}
