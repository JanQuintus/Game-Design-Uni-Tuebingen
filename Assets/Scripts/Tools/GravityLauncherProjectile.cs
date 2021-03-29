using System.Collections.Generic;
using UnityEngine;

public class GravityLauncherProjectile : MonoBehaviour
{
    private const float AIR_LIFE_TIME = 5f;

    [SerializeField] private float growthRate = 20f;
    [SerializeField] private float radius = 20f;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float activeLifeTime = 30f;
    [SerializeField] private bool deactivateLifeTime = false;
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    private float _initialRadius;
    private bool _active = false;
    private float _lifeTime = 0f;
    private Transform _sphere;
    private Transform _plane;
    private Transform _projectile;
    private Transform _projectileParticles;
    private Transform _twirlSystem;
    private Light _light;
    private float _leaveRadius2;
    
    private MeshRenderer _meshRenderer;
    private Rigidbody _rb;
    private List<GravityObject> _objectsInArea = new List<GravityObject>();

    private void Awake()
    {
        _initialRadius = radius;
        _sphere = transform.Find("Sphere");
        _projectile = transform.Find("Projectile");
        _plane = transform.Find("Plane");
        _projectileParticles = transform.Find("ProjectileParticles");
        _twirlSystem = transform.Find("TwirlSystem");
        _light= transform.Find("Light").GetComponent<Light>();
        _sphere.localScale = new Vector3(0, 0, 0);

        _meshRenderer = _projectile.GetComponent<MeshRenderer>();
        _rb = GetComponent<Rigidbody>();
        _leaveRadius2 = (radius + 4f) * (radius + 4f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!deactivateLifeTime)
        {
            _lifeTime += Time.deltaTime;
        }

        if (!_active)
        {
            //transform.forward = _rb.velocity.normalized;
            transform.rotation = Quaternion.LookRotation(_rb.velocity.normalized, transform.up);

            float l = _rb.velocity.magnitude / 10f;
            Vector3 skew = new Vector3(
               Mathf.Min(2f, Mathf.Max(0.1f, 1f / ((4f / 3f) * Mathf.PI * l))),
               Mathf.Min(2f, Mathf.Max(0.1f, 1f / ((4f / 3f) * Mathf.PI * l))),
               l);

            _meshRenderer.material.SetVector("_Velocity", skew);
          
            if (_lifeTime >= AIR_LIFE_TIME)
                Destroy(gameObject);
        }
        else
        {
            if(_lifeTime >= activeLifeTime)
            {
                Destroy(gameObject);
                return;
            }    

            Collider[] co = Physics.OverlapSphere(transform.position, radius, layerMask);
            foreach (Collider collider in co)
            {
                GravityObject gravityObject = Utils.FindGravityObject(collider);
                if (!gravityObject)
                    continue;
                if (_objectsInArea.Contains(gravityObject))
                    continue;
                (bool affected, _) = IsAffactingObject(gravityObject.GetMainCollider().transform, false);
                if (affected)
                    _objectsInArea.Add(gravityObject);
            }

            List<GravityObject> toRemove = new List<GravityObject>();
            foreach (GravityObject go in _objectsInArea)
            {
                if(!go.GetMainCollider())
                {
                    toRemove.Add(go);
                    continue;
                }
                (bool affected, bool resetGravity) = IsAffactingObject(go.GetMainCollider().transform, true);
                if (affected)
                    go.SetLocalGravity(transform.up * -9.81f);
                else
                {
                    if(resetGravity)
                        go.ResetGravity();
                    toRemove.Add(go);
                }
            }
            foreach(GravityObject rem in toRemove)
                _objectsInArea.Remove(rem);
        }

        if (_active)
        {
            _sphere.transform.localScale = Vector3.one * radius * (1f - _lifeTime / activeLifeTime);
            _plane.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f) * radius * (1f - _lifeTime / activeLifeTime);
            _twirlSystem.localScale = Vector3.one * radius * (1f - _lifeTime / activeLifeTime);
            _projectileParticles.localScale = Vector3.one * radius * (1f - _lifeTime / activeLifeTime);
            _light.intensity = 10f * (1f - _lifeTime / activeLifeTime);
            _meshRenderer.material.SetVector("_Velocity", new Vector4(.8f * (1f - _lifeTime / activeLifeTime), 0.1f * (1f - _lifeTime / activeLifeTime), .8f * (1f - _lifeTime / activeLifeTime)));
            radius = _initialRadius * (1f - _lifeTime / activeLifeTime);
            _leaveRadius2 = (radius + 4f) * (radius + 4f);
            audioSource.volume = (1f - _lifeTime / activeLifeTime);
        }

    }

    private (bool, bool) IsAffactingObject(Transform trans, bool wasAffected)
    {
        double direction = Vector3.Dot(transform.up, (trans.position - transform.position));
        if (direction <= -2f || Vector3.SqrMagnitude(trans.position - transform.position) > _leaveRadius2)
            return (false, true);

        float distSelf = Vector3.SqrMagnitude(trans.position - transform.position);
        if (direction < 0f && !wasAffected)
            return (false, false);

        foreach (GravityLauncherProjectile glp in FindObjectsOfType<GravityLauncherProjectile>())
        {
            if (glp == this) continue;
            float dist = Vector3.SqrMagnitude(trans.position - glp.transform.position);
            if (dist <= distSelf)
                return (false, false);
        }

        return (true, false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Sticky"))
        {
            _rb.isKinematic = true;
            gameObject.isStatic = true;
            _active = true;
            _lifeTime = 0;
            _sphere.transform.position = transform.position;
            Vector3 n = collision.GetContact(0).normal;
            transform.up = n;
            _projectile.up = n;
            _meshRenderer.material.SetVector("_Velocity", new Vector4(.8f, 0.1f, .8f));
            _sphere.gameObject.SetActive(true);
            _plane.gameObject.SetActive(true);
            _twirlSystem.gameObject.SetActive(true);
            _projectileParticles.gameObject.SetActive(true);
            _meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            Destroy(GetComponent<GravityObject>());
            Destroy(GetComponent<Collider>());
            Destroy(_rb);
            audioSource.Play();
        }
    }

    private void OnDestroy()
    {
        Destroy(_sphere);
        foreach (GravityObject go in _objectsInArea)
            go.ResetGravity();
        _objectsInArea.Clear();
    }

    public void SetDeactivateLifeTime(bool active)
    {
        deactivateLifeTime = active;
    }
}
