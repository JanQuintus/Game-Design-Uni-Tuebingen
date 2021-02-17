using System.Collections.Generic;
using UnityEngine;

public class GravityLauncherTool : ATool
{
    [SerializeField] private int maxAmmo = 25;
    [SerializeField] private float shootDelay = 0.25f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootClip;

    GameObject _projectilePrefab;
    List<GameObject> _shotProjectiles = new List<GameObject>();
    private int _ammo = 25;
    private float _t = 0;

    private void Awake()
    {
        _projectilePrefab = Resources.Load("Projectile") as GameObject;
    }

    private void Update()
    {
        if(_t > 0)
        {
            _t -= Time.deltaTime;
            if (_t <= 0f)
                _t = 0f;
        }
    }

    public override void Shoot(Ray ray, bool isRelease = false, bool isRightClick = false)
    {
        if (isRelease || isRightClick || _ammo <= 0 || _t > 0f) return;

        _ammo--;
        OnFillChanged?.Invoke();
        GameObject projectile = Instantiate(_projectilePrefab);
        projectile.transform.right = ray.direction;
        projectile.name = "GravityBomb";
        projectile.transform.position = transform.position + Camera.main.transform.forward * 2;
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        _shotProjectiles.Add(projectile);
        rb.velocity = ray.direction * 20;
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(shootClip, Random.Range(0.5f, 1f));
        _t = shootDelay;
        
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

    public override float GetFillPercentage()
    {
        return (float)_ammo / (float)maxAmmo;
    }

    public override float GetFill()
    {
        return _ammo;
    }

    public override void SetFill(float fill)
    {
        _ammo = (int)fill;
        OnFillChanged?.Invoke();
    }
}
