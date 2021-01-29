using UnityEngine;
using System.Collections.Generic;

public class MassExchangerTool : ATool
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private int maxEnergy = 20;
    [SerializeField] private GameObject beamHit;
    [SerializeField] private Transform beamStart;
    [SerializeField] private LineRenderer laserEffect_IN;
    [SerializeField] private LineRenderer laserEffect_OUT;
    [SerializeField] private GameObject massExChangeEffect_In;
    [SerializeField] private GameObject massExChangeEffect_Out;
    [SerializeField] private GameObject chargerEffect;
    [SerializeField] private GameObject _inst_FX_hitFX;

    // initialize 2 vars to keep "saved" rb masses from objects with -1
    public float mEMassLeft = -1.0f;
    public float mEMassRight = -1.0f;

    // initialize 2 objects that keep "saved" objects so we can swap them later
    public Rigidbody mERightObj;
    public Rigidbody mELeftObj;

    // initialize 2 stacks that store default values of objects so we can reset them
    private Stack<Rigidbody> mEObjStack = new Stack<Rigidbody>();
    private Stack<float> mEMassStack = new Stack<float>();

    // initialize energy ( ammo )
    public int mEEnergy = 20;

    //Shader FX
    private int _shader_active = Shader.PropertyToID("massExchangerActive");
    private int _shader_Mass = Shader.PropertyToID("massExchangerMass");
    private int _shader_Mass_For_Rings = Shader.PropertyToID("massExchanger_Rings_Mass");
    

    public override void Shoot(Ray ray, bool isRelease = false, bool isRightClick = false)
    {
        if (isRelease)
            return;

        // Raycast
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, layerMask))
        {
            // depending on what kinda click
            if (isRightClick) // rite click
            {
                Rigidbody rb = Utils.findRigidbody(hit.collider);
                // is the hit object of type rigidbody?
                if (rb != null)
                {
                    // store ref to object
                    mERightObj = rb;

                    // store mass
                    mEMassRight = Mathf.Round(hit.transform.gameObject.GetComponent<Rigidbody>().mass);
                    fxActivate_getRay(hit, laserEffect_IN, mEMassRight);

                    // if other mass isn't -1 ( the default/unassigned value ), ignore swapping the masses of the same object, have ammo
                    if ((mEMassLeft != -1) && (mERightObj != mELeftObj) && (mEEnergy > 0))
                    {
                        
                        // add values to the stack so we can reset them later
                        mEObjStack.Push(mERightObj.GetComponent<Rigidbody>());
                        mEMassStack.Push(mEMassRight);
                        mEObjStack.Push(mELeftObj.GetComponent<Rigidbody>());
                        mEMassStack.Push(mEMassLeft);

                        // add this mass to other obj, add other mass to this obj, reset stored values to -1, deduct ammo
                        mELeftObj.GetComponent<Rigidbody>().mass = mEMassRight;
                        mERightObj.GetComponent<Rigidbody>().mass = mEMassLeft;
                        mEEnergy -= 1;

                        mEMassLeft = -1;
                        mEMassRight = -1;
                    }
                }
            }
            else // teh other click
            {
                Rigidbody rb = Utils.findRigidbody(hit.collider);
                // is the hit object of type rigidbody?
                if (rb != null)
                {
                    // store ref to object
                    mELeftObj = rb;

                    // store mass
                    mEMassLeft = Mathf.Round(rb.mass);

                    fxActivate_shootRay(hit, laserEffect_OUT, mEMassRight);
                    // if other mass isn't -1 ( the default/unassigned value ), ignore swapping the masses of the same object, have ammo
                    if ((mEMassRight != -1) && (mERightObj != mELeftObj) && (mEEnergy > 0))
                    {
                        
                        // add values to the stack so we can reset them later
                        mEObjStack.Push(mERightObj.GetComponent<Rigidbody>());
                        mEMassStack.Push(mEMassRight);
                        mEObjStack.Push(mELeftObj.GetComponent<Rigidbody>());
                        mEMassStack.Push(mEMassLeft);

                        // add this mass to other obj, add other mass to this obj, reset stored values to -1, deduct ammo
                        mERightObj.GetComponent<Rigidbody>().mass = mEMassLeft;
                        mELeftObj.GetComponent<Rigidbody>().mass = mEMassRight;
                        mEEnergy -= 1;

                        mEMassLeft = -1;
                        mEMassRight = -1;
                        
                    }
                }
            }
        }
    }

    private void SwitchMasses()
    {
        // add values to the stack so we can reset them later
        mEObjStack.Push(mELeftObj);
        mEMassStack.Push(mEMassRight);
        mEObjStack.Push(mERightObj);
        mEMassStack.Push(mEMassLeft);

        // add this mass to other obj, add other mass to this obj, reset stored values to -1, deduct ammo
        mELeftObj.mass = mEMassRight;
        mERightObj.mass = mEMassLeft;
        mEEnergy -= 1;
        OnFillChanged?.Invoke();

        mEMassLeft = -1;
        mEMassRight = -1;
    }

    public override void Reset(bool isRelease)
    {

        // as long as there are elements in the stack(s):
        while (mEObjStack.Count > 0)
        {
            mEObjStack.Peek().GetComponent<Renderer>().material.SetFloat(_shader_active, 0);
            // mass of previously stored rigidbody is previously stored float ( thanks lifo - first stored values will be restored last, loop basically resets each swap individually (perf?) )
            mEObjStack.Pop().mass = mEMassStack.Pop();
  
        }
    }

    public override void Reload() { 
        mEEnergy = maxEnergy;
        OnFillChanged?.Invoke();
    }

    public override void Scroll(float delta){}

    public override void OnEquip()
    { }

    public override void OnUnequip()
    { }

    private void updateAllColor(GameObject input, int propertyID, float mass) {

        Renderer[] children;
        children = input.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in children)
        {
            rend.material.SetFloat(propertyID, mass);
        }

    }

    private void fxActivate_getRay(RaycastHit hit, LineRenderer laser, float setMass)
    {
        chargerEffect.SetActive(true);
        massExChangeEffect_In.SetActive(true);

        _inst_FX_hitFX = Instantiate(beamHit, hit.point, Quaternion.LookRotation(hit.normal));
        laserEffect_IN.SetPosition(1, transform.InverseTransformPoint(hit.point));
        laserEffect_IN.SetPosition(0, transform.InverseTransformPoint(beamStart.transform.position));

        //Shader FX
        //updateAllColor(hit.collider.gameObject, _shader_active, 1);
        //updateAllColor(hit.collider.gameObject, _shader_Mass, mEMassRight);
        updateAllColor(chargerEffect, _shader_Mass_For_Rings, setMass);

    }

    private void fxActivate_shootRay(RaycastHit hit, LineRenderer laser, float setMass)
    {
        chargerEffect.SetActive(false);
        massExChangeEffect_Out.SetActive(true);

        _inst_FX_hitFX = Instantiate(beamHit, hit.point, Quaternion.LookRotation(hit.normal));
        laserEffect_OUT.SetPosition(1, transform.InverseTransformPoint(hit.point));
        laserEffect_OUT.SetPosition(0, transform.InverseTransformPoint(beamStart.transform.position));

        //Shader FX
        updateAllColor(hit.collider.gameObject, _shader_active, 1);
        updateAllColor(hit.collider.gameObject, _shader_Mass, setMass);
        updateAllColor(chargerEffect, _shader_Mass_For_Rings, setMass);
    }

}