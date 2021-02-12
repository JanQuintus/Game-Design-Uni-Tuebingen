using System.Collections.Generic;
using UnityEditor;
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

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootClip;

    // Shaderchange VFX
    private int _activateEmssionID = Shader.PropertyToID("gunActive");

    private float _energy = 100;
    private List<GravityObject> _changedObjects = new List<GravityObject>();
    private Dictionary<GravityObject, GameObject> _vfxOnBodies = new Dictionary<GravityObject, GameObject>();

    private void Update()
    {
        if (_changedObjects.Count > 0) {
            _energy -= Time.deltaTime * _changedObjects.Count;
            OnFillChanged?.Invoke();
            if (_energy <= 0)
            {
                _energy = 0;
                Reset(false);
            }

            foreach (KeyValuePair<GravityObject, GameObject> vfxOnBody in _vfxOnBodies)
            {
                vfxOnBody.Value.transform.position = vfxOnBody.Key.GetMainCollider().transform.position;
                vfxOnBody.Value.transform.rotation = vfxOnBody.Key.GetMainCollider().transform.rotation;
            }
        }
    }

    public override void Shoot(Ray ray, bool isRelease = false, bool isRightClick = false)
    {
        if (isRelease || isRightClick) return;
        if (_energy <= 0) return;
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, layerMask))
        {
            GravityObject go = Utils.FindGravityObject(hit.collider);
            if (go)
            {
                go.GetRB().useGravity = !go.GetRB().useGravity;
                fxActivate(hit);
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(shootClip, Random.Range(0.5f, 1f));

                if (_changedObjects.Contains(go))
                {
                    _changedObjects.Remove(go);
                    //Shader FX
                    Utils.SetMaterialPropertyFloat(go.GetMainRenderer(), _activateEmssionID, 0);
                    // Particle FX
                    if(_vfxOnBodies.TryGetValue(go, out GameObject vfx))
                    {
                        Destroy(vfx);
                        _vfxOnBodies.Remove(go);
                    }
                }
                else
                {
                    _changedObjects.Add(go);
                    Utils.SetMaterialPropertyFloat(go.GetMainRenderer(), _activateEmssionID, 1);
                    //Particle FX
                    GameObject inst_FX = Instantiate(gravityEffect, go.GetMainCollider().transform.position, go.GetMainCollider().transform.rotation);
                    _vfxOnBodies.Add(go, inst_FX);
                    // set FX size
                    Vector3 rb_size = hit.collider.bounds.size;
                    inst_FX.transform.localScale *= rb_size.magnitude;

                }
            }
        }
    }

    public override void Reset(bool isRelease)
    {
        if (!isRelease)
        {
            foreach (GravityObject go in _changedObjects)
            {
                go.GetRB().useGravity = !go.GetRB().useGravity;
                Utils.SetMaterialPropertyFloat(go.GetMainRenderer(), _activateEmssionID, 0);
            }

            foreach (GameObject FX in _vfxOnBodies.Values)
            {
                Destroy(FX, 0.05f);
            }

            _changedObjects.Clear();
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

    public override float GetFillPercentage()
    {
        return _energy / maxEnergy;
    }

    private void fxActivate(RaycastHit hit)
    {
        //RayActivate
        gravityGunRay.SetActive(true);
        gravityGunRay_Laser.SetPosition(1, transform.InverseTransformPoint(hit.point));
        gravityGunRay_Laser.SetPosition(0, transform.InverseTransformPoint(beamStart.transform.position));
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
