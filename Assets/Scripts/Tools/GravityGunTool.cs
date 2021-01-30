using System.Collections.Generic;
using UnityEngine;

public class GravityGunTool : ATool
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float distance;
    [SerializeField] private float maxEnergy = 100;
    [SerializeField] private GameObject gravityGunRay;
    [SerializeField] private LineRenderer gravityGunRay_Laser;
    [SerializeField] private Transform beamStart;
    [SerializeField] private GameObject gravityEffect;
    [SerializeField] private GameObject _inst_FX;

    // Shaderchange VFX
    private int _activateEmssionID = Shader.PropertyToID("gunActive");

    private float _energy = 100;
    private List<Rigidbody> _changedBodies = new List<Rigidbody>();
    private List<GameObject> _vfxOnBodies = new List<GameObject>();

    private float _lastClicked = 0;

    private void Update()
    {
        if (_changedBodies.Count > 0) {
            _energy -= Time.deltaTime * _changedBodies.Count;
            OnFillChanged?.Invoke();
            if (_energy <= 0)
            {
                _energy = 0;
                Reset(false);
            }

            for (int index = 0; index < _changedBodies.Count; index++)
            {
                Rigidbody rb = _changedBodies[index];
                _vfxOnBodies[index].transform.position = rb.transform.position;
                _vfxOnBodies[index].transform.rotation = rb.transform.rotation;
            }


        }

    }

    public override void Shoot(Ray ray, bool isRelease = false, bool isRightClick = false)
    {
        if (isRelease || isRightClick) return;
        if (_energy <= 0) return;
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, layerMask))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

            
            if (rb)
            {
                rb.useGravity = !rb.useGravity;
                fxActivate(hit);
                // ShaderChange
                Material material = hit.collider.gameObject.GetComponent<Renderer>().material;


                if (_changedBodies.Contains(rb))
                {
                    _changedBodies.Remove(rb);
                    //Shader FX
                    material.SetFloat(_activateEmssionID, 0);
                    // Particle FX
                    Destroy(_inst_FX);
                }
                else
                {
                    _changedBodies.Add(rb);
                    material.SetFloat(_activateEmssionID, 1);
                    //Particle FX
                    _inst_FX = Instantiate(gravityEffect, rb.position, rb.rotation);
                    _vfxOnBodies.Add(_inst_FX);
                    // set FX size
                    Vector3 rb_size = hit.collider.bounds.size;
                    _inst_FX.transform.localScale *= rb_size.magnitude;

                }
            }
        }
    }

    public override void Reset(bool isRelease)
    {
        if (!isRelease)
        {
            foreach (Rigidbody rb in _changedBodies)
            {
                rb.useGravity = !rb.useGravity;
                rb.gameObject.GetComponent<Renderer>().material.SetFloat(_activateEmssionID, 0);
            }

            foreach (GameObject FX in _vfxOnBodies)
            {
                Destroy(FX, 0.05f);
            }

            _changedBodies.Clear();
            _vfxOnBodies.Clear();
        }
    }

    public override void Reload(){
        _energy = maxEnergy;
        OnFillChanged?.Invoke();
    }

    public override void Scroll(float delta) { }

    public override void OnEquip()
    { }

    public override void OnUnequip()
    {}

    public override float getFillPercentage()
    {
        return _energy / maxEnergy;
    }

    private void fxActivate(RaycastHit hit)
    {
        //RayActivate
        gravityGunRay.SetActive(true);
        _lastClicked = Time.time;
        gravityGunRay_Laser.SetPosition(1, transform.InverseTransformPoint(hit.point));
        gravityGunRay_Laser.SetPosition(0, transform.InverseTransformPoint(beamStart.transform.position));
    }
}
