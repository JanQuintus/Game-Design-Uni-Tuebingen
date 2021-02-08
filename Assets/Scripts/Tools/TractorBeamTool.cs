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

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip beamOnClip;
    [SerializeField] private AudioClip beamLoop;
    [SerializeField] private AudioClip beamOffClip;
    [SerializeField] private AudioClip catchObjectClip;
    [SerializeField] private AudioClip shootObjectClip;
    [SerializeField] private AudioClip windupClip;

    private float _heat = 0;
    private bool _windUp;
    private float _shootForce = 1; // current shoot force

    private void Awake()
    {
        TheBeam.OnCatchObject += () => audioSource.PlayOneShot(catchObjectClip);
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
            TurnOffBeam();
    }

    public override void Shoot(Ray ray, bool isRelease = false, bool isRightClick = false)
    {

        //rmb pressed
        if (isRightClick && !isRelease)
            TurnOnBeam();

        //rmb released
        if (isRightClick && isRelease)
            TurnOffBeam();

        //lmb released
        if (!isRightClick && isRelease)
        {
            if (!TheBeam.IsActive())
                return;
            //shoot object in beam
            Vector3 shootDirection = ray.direction;
            TheBeam.shoot(_shootForce, shootDirection);
            TurnOffBeam();
            audioSource.PlayOneShot(shootObjectClip, 0.25f);

            // reset windup
            _windUp = false;
            _shootForce = 1;
        }

        //lmb pressed
        if (!isRightClick && !isRelease)
        {
            if (!TheBeam.IsActive())
                return;
            _windUp = true;
            audioSource.PlayOneShot(windupClip);
        }
    }

    private void TurnOnBeam()
    {
        audioSource.pitch = 1;
        TheBeam.turnOnBeam(range);
        audioSource.PlayOneShot(beamOnClip, 0.25f);
        audioSource.clip = beamLoop;
        audioSource.loop = true;
        audioSource.Play();
    }
    private void TurnOffBeam()
    {
        if (!TheBeam.IsActive())
            return;
        TheBeam.turnOffBeam();
        audioSource.Stop();
        audioSource.clip = null;
        audioSource.loop = false;
        audioSource.PlayOneShot(beamOffClip, 0.25f);
    }

    public override void Scroll(float delta)
    {
        TheBeam.changeDisplacementIntensity(delta);
    }

    public override void Reload() {}

    public override void Reset(bool isRelease) {}

    public override void OnEquip() { }

    public override void OnUnequip() => TurnOffBeam();

    public override float getFillPercentage()
    {
        return 1f - (_heat / maximalHeat);
    }
}
