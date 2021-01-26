using System.Collections.Generic;
using UnityEngine;

public class GravityGunTool : ATool
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float distance;
    [SerializeField] private float maxEnergy = 100;

    private float _energy = 100;
    private List<Rigidbody> _changedBodies = new List<Rigidbody>();

    private void Update()
    {
        if (_changedBodies.Count > 0){
            _energy -= Time.deltaTime * _changedBodies.Count;
            if (_energy <= 0)
            {
                _energy = 0;
                Reset(false);
            }
        }
    }

    public override void Shoot(Ray ray, bool isRelease = false, bool isRightClick = false)
    {
        if (isRelease || isRightClick) return;
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, layerMask))
        {
            Rigidbody rb = Utils.findRigidbody(hit.collider);
            if (rb)
            {
                rb.useGravity = !rb.useGravity;
                if(_changedBodies.Contains(rb))
                {
                    _changedBodies.Remove(rb);
                }
                else _changedBodies.Add(rb);
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
            }
            _changedBodies.Clear();
        }
    }

    public override void Reload() => _energy = maxEnergy;

    public override void Scroll(float delta) { }

    public override void OnEquip()
    {}

    public override void OnUnequip()
    {}
}
