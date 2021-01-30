using System.Collections.Generic;
using UnityEngine;

public class GravityLauncherTool : ATool
{
    [SerializeField] private int maxAmmo = 25;
    GameObject _projectilePrefab;
    List<GameObject> _shotProjectiles = new List<GameObject>();
    private int _ammo = 25;

    private void Awake()
    {
        _projectilePrefab = Resources.Load("Projectile") as GameObject;
    }

    public override void Shoot(Ray ray, bool isRelease = false, bool isRightClick = false)
    {
        if (isRelease || isRightClick) return;
        // Check if a projectile is already active, if so destroy it. (Can be done on right and left click)

        if (!isRightClick && _ammo > 0)
        {
            _ammo--;
            OnFillChanged?.Invoke();
            GameObject projectile = Instantiate(_projectilePrefab);
            projectile.transform.right = ray.direction;
            projectile.name = "GravityBomb";
            projectile.transform.position = transform.position + Camera.main.transform.forward * 2;
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            _shotProjectiles.Add(projectile);
            rb.velocity = ray.direction * 20;
        }
    }
    
    public override void Reset(bool isRelease) {
        if(!isRelease)
        {
            foreach(GameObject projectile in _shotProjectiles)
            {
                Destroy(projectile);
            }
            _shotProjectiles.Clear();
        }
    }

    public override void Reload() { 
        _ammo = maxAmmo;
        OnFillChanged?.Invoke();
    }

    public override void Scroll(float delta){}

    public override void OnEquip()
    { }

    public override void OnUnequip()
    { }

    public override float getFillPercentage()
    {
        return (float)_ammo / (float)maxAmmo;
    }
}
