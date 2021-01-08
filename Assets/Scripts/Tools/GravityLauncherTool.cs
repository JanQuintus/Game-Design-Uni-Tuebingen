using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityLauncherTool : ATool
{
    [SerializeField] private int maxAmmo = 25;
    GameObject _projectile;
    GameObject _bullet;
    GameObject _newBullet;
    private int _ammo = 25;

    private void Awake()
    {
        _projectile = Resources.Load("Projectile") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (_newBullet)
        {
            if (_newBullet.GetComponent<GravityLauncherProjectile>().getIsLocked())
            {
                if (_bullet) Destroy(_bullet);
                _bullet = _newBullet;
                _bullet.GetComponent<GravityLauncherProjectile>().setIsLocked(false);
            }
        }
    }

    public override void Shoot(Ray ray, bool isRelease = false, bool isRightClick = false)
    {
        if (isRelease || isRightClick) return;
        // Check if a projectile is already active, if so destroy it. (Can be done on right and left click)

        if (!isRightClick && _ammo > 0)
        {
            _ammo--;
            if (_newBullet)
            {
                if (!_newBullet.GetComponent<GravityLauncherProjectile>().getActive())
                {
                    Destroy(_newBullet);
                }
            }
            _newBullet = Instantiate(_projectile);
            _newBullet.transform.right = ray.direction;
            _newBullet.name = "GravityBomb";
            _newBullet.transform.position = transform.position + Camera.main.transform.forward * 2;
            Rigidbody rb = _newBullet.GetComponent<Rigidbody>();
            rb.velocity = ray.direction * 20;
        }
    }

   
    
    public override void Reset(bool isRelease) {
        if(!isRelease)
        {
            if (_bullet) Destroy(_bullet);
            if (_newBullet) Destroy(_newBullet);
        }
    }

    public override void Reload() => _ammo = maxAmmo;

    public override void Scroll(float delta){}


}
