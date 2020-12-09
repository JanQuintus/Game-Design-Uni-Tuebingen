using UnityEngine;

public class MassExchangerTool : ATool
{
    // initialize 2 vars to keep "saved" rb masses from objects with -1
    public float mEMassLeft = -1.0f;
    public float mEMassRight = -1.0f;

    // initialize 2 objects that keep "saved" objects so we can swap them later
    public GameObject mERightObj;
    public GameObject mELeftObj;


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

                    // if other mass isn't -1 ( the default/unassigned value ), ignore swapping the masses of the same object
                    if ((mEMassLeft != -1) && (mERightObj != mELeftObj))
                    {
                        // add this mass to other obj, add other mass to this obj, reset stored values to -1
                        mELeftObj.GetComponent<Rigidbody>().mass = mEMassRight;
                        mERightObj.GetComponent<Rigidbody>().mass = mEMassLeft;

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

                    // if other mass isn't -1 ( the default/unassigned value ), ignore swapping the masses of the same object
                    if ((mEMassRight != -1) && (mERightObj != mELeftObj))
                    {
                        // add this mass to other obj, add other mass to this obj, reset stored values to -1
                        mERightObj.GetComponent<Rigidbody>().mass = mEMassLeft;
                        mELeftObj.GetComponent<Rigidbody>().mass = mEMassRight;

                        mEMassLeft = -1;
                        mEMassRight = -1;
                    }
                }
            }
        }
    }

    public override void Reset(bool isRelease)
    {
        
    }

    public override void Scroll(float delta){}
}