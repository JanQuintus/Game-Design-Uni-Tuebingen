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

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip soakClip;

    // initialize 2 objects that keep "saved" objects so we can swap them later
    private GravityObject _source;
    private GravityObject _target;

    // initialize 2 stacks that store default values of objects so we can reset them
    private Dictionary<GravityObject, float> _changedObjects = new Dictionary<GravityObject, float>();

    // initialize energy ( ammo )
    private float _energy = 20;

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
            GravityObject go = Utils.FindGravityObject(hit.collider);
            if (go == null)
                return;

            if (isRightClick)
            {
                _source = go;
                fxActivate_getRay(hit);
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(shootClip, Random.Range(0.5f, 1f));
            }
            else
            {
                if (_source == null)
                    return;

                _target = go;
                fxActivate_shootRay(hit);
                SwitchMasses();
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(shootClip, Random.Range(0.5f, 1f));
            }
        }
    }

    private void SwitchMasses()
    {
        if(!_changedObjects.ContainsKey(_source))
            _changedObjects.Add(_source, _source.GetRB().mass);
        if (!_changedObjects.ContainsKey(_target))
            _changedObjects.Add(_target, _target.GetRB().mass);
        float sourceMass = _source.GetRB().mass;
        _source.GetRB().mass = _target.GetRB().mass;
        _target.GetRB().mass = sourceMass;
    }

    public override void Reset(bool isRelease)
    {
        foreach(KeyValuePair<GravityObject, float> changedObject in _changedObjects)
        {
            changedObject.Key.GetRB().mass = changedObject.Value;
            Utils.SetMaterialPropertyFloat(changedObject.Key.GetMainRenderer(), _shader_active, 0);
            Utils.SetMaterialPropertyFloat(changedObject.Key.GetMainRenderer(), _shader_Mass, _target.GetRB().mass);
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

    private void fxActivate_getRay(RaycastHit hit)
    {
        chargerEffect.SetActive(true);
        massExChangeEffect_In.SetActive(true);

        _inst_FX_hitFX = Instantiate(beamHit, hit.point, Quaternion.LookRotation(hit.normal));
        laserEffect_IN.SetPosition(1, transform.InverseTransformPoint(hit.point));
        laserEffect_IN.SetPosition(0, transform.InverseTransformPoint(beamStart.transform.position));

        Renderer[] children = chargerEffect.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in children)
            rend.material.SetFloat(_shader_Mass_For_Rings, _source.GetRB().mass);
    }

    private void fxActivate_shootRay(RaycastHit hit)
    {
        chargerEffect.SetActive(false);
        massExChangeEffect_Out.SetActive(true);

        _inst_FX_hitFX = Instantiate(beamHit, hit.point, Quaternion.LookRotation(hit.normal));
        laserEffect_OUT.SetPosition(1, transform.InverseTransformPoint(hit.point));
        laserEffect_OUT.SetPosition(0, transform.InverseTransformPoint(beamStart.transform.position));

        //Shader FX
        Utils.SetMaterialPropertyFloat(_source.GetMainRenderer(), _shader_active, 1);
        Utils.SetMaterialPropertyFloat(_source.GetMainRenderer(), _shader_Mass, _target.GetRB().mass);
        Utils.SetMaterialPropertyFloat(_target.GetMainRenderer(), _shader_active, 1);
        Utils.SetMaterialPropertyFloat(_target.GetMainRenderer(), _shader_Mass, _source.GetRB().mass);
    }

    public override float GetFillPercentage()
    {
        return _energy / maxEnergy;
    }

    public override float GetFill()
    {
        return _energy;
    }

    public override void SetFill(float fill)
    {
        _energy = fill;
        OnFillChanged?.Invoke();
    }
}