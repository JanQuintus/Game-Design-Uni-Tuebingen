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
            if(energy <= 0)
            {
                changedBodies.Clear();
            }
        }
    }

    public override void Shoot(Ray ray, bool isRelease = false, bool isRightClick = false)
    {
        if (isRightClick || isRelease)
            return;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                changedBodies.Add(rb);
            }
        }
    }

    public override void Reset(bool isRelease)
    {
        
    }

    public override void Scroll(float delta) { }
}
