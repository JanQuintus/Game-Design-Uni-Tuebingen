using UnityEngine;
using System.Collections.Generic;

public class MassExchangerTool : ATool
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float maxEnergy = 20;
    [SerializeField] private GameObject beamHit;
    [SerializeField] private Transform beamStart;
    [SerializeField] private LineRenderer laserEffect_IN;
    [SerializeField] private LineRenderer laserEffect_OUT;
    [SerializeField] private GameObject massExChangeEffect_In;
    [SerializeField] private GameObject massExChangeEffect_Out;
    [SerializeField] private GameObject chargerEffect;
    [SerializeField] private GameObject _inst_FX_hitFX;

    // initialize 2 objects that keep "saved" objects so we can swap them later
    public Rigidbody _rbSource;
    public Rigidbody _rbTarget;

    // initialize 2 stacks that store default values of objects so we can reset them
    private Dictionary<Rigidbody, float> _changedObjects = new Dictionary<Rigidbody, float>();

    // initialize energy ( ammo )
    public float _energy = 20;

    //Shader FX
    private int _shader_active = Shader.PropertyToID("massExchangerActive");
    private int _shader_Mass = Shader.PropertyToID("massExchangerMass");
    private int _shader_Mass_For_Rings = Shader.PropertyToID("massExchanger_Rings_Mass");

    private void Awake()
    {
        _energy = maxEnergy;
    }

    private void Update()
    {
        if (_changedObjects.Count > 0)
        {
            _energy -= Time.deltaTime * _changedObjects.Count;
            OnFillChanged?.Invoke();
            if (_energy <= 0)
            {
                _energy = 0;
                Reset(false);
            }
        }
    }

    public override void Shoot(Ray ray, bool isRelease = false, bool isRightClick = false)
    {
        if (isRelease) return;
        if (_energy <= 0) return;

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, layerMask))
        {
            Rigidbody rb = Utils.findRigidbody(hit.collider);
            if (rb == null)
                return;

            if (isRightClick)
            {
                _rbSource = rb;
                fxActivate_getRay(hit);
            }
            else
            {
                if (_rbSource == null)
                    return;

                _rbTarget = rb;
                fxActivate_shootRay(hit);
                SwitchMasses();
            }
        }
    }

    private void SwitchMasses()
    {
        if(!_changedObjects.ContainsKey(_rbSource))
            _changedObjects.Add(_rbSource, _rbSource.mass);
        if (!_changedObjects.ContainsKey(_rbTarget))
            _changedObjects.Add(_rbTarget, _rbTarget.mass);
        float sourceMass = _rbSource.mass;
        _rbSource.mass = _rbTarget.mass;
        _rbTarget.mass = sourceMass;
    }

    public override void Reset(bool isRelease)
    {
        foreach(KeyValuePair<Rigidbody, float> changedObject in _changedObjects)
        {
            changedObject.Key.mass = changedObject.Value;
            updateAllColor(changedObject.Key.gameObject, _shader_active, 0);
            updateAllColor(changedObject.Key.gameObject, _shader_Mass, _rbTarget.mass);
        }
        _changedObjects.Clear();
    }

    public override void Reload() { 
        _energy = maxEnergy;
        OnFillChanged?.Invoke();
    }

    public override void Scroll(float delta){}
    public override void OnEquip() { }
    public override void OnUnequip() { }

    private void updateAllColor(GameObject input, int propertyID, float mass) {

        Renderer[] children;
        children = input.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in children)
            rend.material.SetFloat(propertyID, mass);
    }

    private void fxActivate_getRay(RaycastHit hit)
    {
        chargerEffect.SetActive(true);
        massExChangeEffect_In.SetActive(true);

        _inst_FX_hitFX = Instantiate(beamHit, hit.point, Quaternion.LookRotation(hit.normal));
        laserEffect_IN.SetPosition(1, transform.InverseTransformPoint(hit.point));
        laserEffect_IN.SetPosition(0, transform.InverseTransformPoint(beamStart.transform.position));

        updateAllColor(chargerEffect, _shader_Mass_For_Rings, _rbSource.mass);
    }

    private void fxActivate_shootRay(RaycastHit hit)
    {
        chargerEffect.SetActive(false);
        massExChangeEffect_Out.SetActive(true);

        _inst_FX_hitFX = Instantiate(beamHit, hit.point, Quaternion.LookRotation(hit.normal));
        laserEffect_OUT.SetPosition(1, transform.InverseTransformPoint(hit.point));
        laserEffect_OUT.SetPosition(0, transform.InverseTransformPoint(beamStart.transform.position));

        //Shader FX
        updateAllColor(_rbSource.gameObject, _shader_active, 1);
        updateAllColor(_rbSource.gameObject, _shader_Mass, _rbTarget.mass);
        updateAllColor(_rbTarget.gameObject, _shader_active, 1);
        updateAllColor(_rbTarget.gameObject, _shader_Mass, _rbSource.mass);
    }

    public override float getFillPercentage()
    {
        return _energy / maxEnergy;
    }
}