using System.Collections.Generic;
using UnityEngine;

public class GravityGunTool : ATool
{
    [SerializeField]
    private float distance;

    float energy = 100;
    List<Rigidbody> changedBodies = new List<Rigidbody>();

    private void Update()
    {
        if (changedBodies.Count > 0){
            energy -= Time.deltaTime * changedBodies.Count;
            if(energy <= 0) Reset(false);
        }
    }

    public override void Shoot(Ray ray, bool isRelease = false, bool isRightClick = false)
    {
        if (isRelease || isRightClick) return;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.useGravity = !rb.useGravity;
                if(changedBodies.Contains(rb))
                {
                    changedBodies.Remove(rb);
                }
                else changedBodies.Add(rb);
            }
        }
    }

    public override void Reset(bool isRelease)
    {
        if (!isRelease)
        {
            foreach (Rigidbody rb in changedBodies)
            {
                rb.useGravity = !rb.useGravity;
            }
            changedBodies.Clear();
        }
    }

    public override void Scroll(float delta) { }


}
