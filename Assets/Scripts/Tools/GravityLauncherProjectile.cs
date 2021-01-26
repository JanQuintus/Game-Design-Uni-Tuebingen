using System.Collections.Generic;
using UnityEngine;

public class GravityLauncherProjectile : MonoBehaviour
{
    [SerializeField] private float growthRate = 20f;
    [SerializeField] private float radius = 20f;
    [SerializeField] private LayerMask layerMask;

    private class ObjectInArea
    {
        public GravityObject Gravity;
        public Collider Col;

        public ObjectInArea(GravityObject gravity, Collider col)
        {
            Gravity = gravity;
            Col = col;
        }
    }

    private bool _isLocked = false;
    private bool _active = false;
    private float _liveTime = 5f;
    private Transform _sphere;
    private Transform _plane;
    private Transform _projectile;
    private Transform _projectileParticles;
    private Transform _twirlSystem;
    private Vector3 _defaultGravity = new Vector3(0, -9.81f, 0);
    private float _leaveRadius2;

    private MeshRenderer _meshRenderer;
    private Rigidbody _rb;
    private Dictionary<GravityObject, ObjectInArea> _objectsInArea = new Dictionary<GravityObject, ObjectInArea>();

    private void Awake()
    {
        _sphere = transform.Find("Sphere");
        _projectile = transform.Find("Projectile");
        _plane = transform.Find("Plane");
        _projectileParticles = transform.Find("ProjectileParticles");
        _twirlSystem = transform.Find("TwirlSystem");
        _sphere.localScale = new Vector3(0, 0, 0);

        _meshRenderer = _projectile.GetComponent<MeshRenderer>();
        _rb = GetComponent<Rigidbody>();
        _leaveRadius2 = (radius + 4f) * (radius + 4f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_active)
        {
            _liveTime -= Time.deltaTime;
            //transform.forward = _rb.velocity.normalized;
            transform.rotation = Quaternion.LookRotation(_rb.velocity.normalized, transform.up);
            Vector3 dir = transform.forward;

            float w = Mathf.Max(0.1f, Mathf.Abs(dir.x) + Mathf.Abs(dir.y));
            float l = Mathf.Max(0.1f, Mathf.Abs(dir.z));
            Vector3 skew = new Vector3(
               1f / ((4f / 3f) * Mathf.PI * w),
               1f / ((4f / 3f) * Mathf.PI * w),
               1f / ((4f / 3f) * Mathf.PI * l));

            _meshRenderer.material.SetVector("_Velocity", skew);
          
            if (_liveTime < 0)
                Destroy(gameObject);
        }
        else
        {
            Collider[] co = Physics.OverlapSphere(transform.position, radius, layerMask);
            foreach (Collider collider in co)
            {
                GravityObject gravityObject = Utils.findGravityObject(collider);
                if (!gravityObject)
                    continue;
                double direction = Vector3.Dot(transform.up, (collider.transform.position - transform.position));
                if (direction > -2 && !_objectsInArea.ContainsKey(gravityObject)) _objectsInArea.Add(gravityObject, new ObjectInArea(gravityObject, collider));
            }

            List<GravityObject> toRemove = new List<GravityObject>();
            foreach (ObjectInArea objInArea in _objectsInArea.Values)
            {
                double direction = Vector3.Dot(transform.up, (objInArea.Col.transform.position - transform.position));
                if (direction <= -2 || Vector3.SqrMagnitude(objInArea.Col.transform.position - transform.position) > _leaveRadius2)
                {
                    objInArea.Gravity.SetLocalGravity(_defaultGravity);
                    toRemove.Add(objInArea.Gravity);
                    continue;
                }
                objInArea.Gravity.SetLocalGravity(transform.up * -9.81f);
            }
            foreach(GravityObject rem in toRemove)
                _objectsInArea.Remove(rem);
        }
        
        if (_active && _sphere.transform.localScale.x < radius)
            _sphere.transform.localScale += Vector3.one * Time.deltaTime * growthRate;
        
        if (_active && _plane.transform.localScale.x < 1)
            _plane.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f) * Time.deltaTime * growthRate;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.isStatic)
        {
            _rb.isKinematic = true;
            _isLocked = true;
            _active = true;
            gameObject.isStatic = true;
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
        }
    }

    public bool getIsLocked() => _isLocked;
    public void setIsLocked(bool locked) => _isLocked = locked;
    public bool getActive() => _active;

    private void OnDestroy()
    {
        Destroy(_sphere);
        foreach (ObjectInArea objInArea in _objectsInArea.Values)
            objInArea.Gravity.SetLocalGravity(_defaultGravity);
        _objectsInArea.Clear();
    }
}
