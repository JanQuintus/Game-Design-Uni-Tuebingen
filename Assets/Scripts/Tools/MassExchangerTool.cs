using UnityEngine;
using System.Collections.Generic;

public class MassExchangerTool : ATool
{
    [SerializeField] private int maxEnergy = 20;

    // initialize 2 vars to keep "saved" rb masses from objects with -1
    public float mEMassLeft = -1.0f;
    public float mEMassRight = -1.0f;

    // initialize 2 objects that keep "saved" objects so we can swap them later
    public GameObject mERightObj;
    public GameObject mELeftObj;

    // initialize 2 stacks that store default values of objects so we can reset them
    private Stack<Rigidbody> mEObjStack = new Stack<Rigidbody>();
    private Stack<float> mEMassStack = new Stack<float>();

    // initialize energy ( ammo )
    public int mEEnergy = 20;

    public override void Shoot(Ray ray, bool isRelease = false, bool isRightClick = false)
    {
        if (isRelease)
            return;

        // Raycast
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // depending on what kinda click
            if (isRightClick) // rite click
            {
                // is the hit object of type rigidbody?
                if (hit.transform.gameObject.GetComponent<Rigidbody>() != null)
                {
                    // store ref to object
                    mERightObj = hit.transform.gameObject;

                    // store mass
                    mEMassRight = Mathf.Round(hit.transform.gameObject.GetComponent<Rigidbody>().mass);

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
                // is the hit object of type rigidbody?
                if (hit.transform.gameObject.GetComponent<Rigidbody>() != null)
                {
                    // store ref to object
                    mELeftObj = hit.transform.gameObject;

                    // store mass
                    mEMassLeft = Mathf.Round(hit.transform.gameObject.GetComponent<Rigidbody>().mass);

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

    public override void Reset(bool isRelease)
    {
        // as long as there are elements in the stack(s):
        while (mEObjStack.Count > 0)
        {
            // mass of previously stored rigidbody is previously stored float ( thanks lifo - first stored values will be restored last, loop basically resets each swap individually (perf?) )
            mEObjStack.Pop().mass = mEMassStack.Pop();
        }
    }

    public override void Reload() => mEEnergy = maxEnergy;

    public override void Scroll(float delta){}

    public override void OnEquip()
    { }

    public override void OnUnequip()
    { }
}