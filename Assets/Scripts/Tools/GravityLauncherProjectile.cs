using eDmitriyAssets.NavmeshLinksGenerator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GravityLauncherProjectile : MonoBehaviour
{
    // Start is called before the first frame update
    private bool _isLocked = false;
    private bool _active = false;
    private float _liveTime = 5f;
    private GameObject _sphere;
    private GameObject _sphereMesh;
    private Vector3 _normal;
    private Vector3 _defaultGravity = new Vector3(0, -9.81f, 0);
    public float radius = 20f;
    private Quaternion _initalRot;
    private NavMeshSurface _navMeshSurface;
    private NavMeshLinks_AutoPlacer _navMeshLinks;

    private MeshRenderer _meshRenderer; 
    [SerializeField] private float growthRate = 20f;

    private void Awake()
    {
        _sphere = transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        _sphere.SetActive(false);
        _sphere.transform.localScale = new Vector3(0, 0, 0);
        _navMeshSurface = GetComponent<NavMeshSurface>();
        _navMeshLinks = GetComponent<NavMeshLinks_AutoPlacer>();

    }
    

    void Start()
    {
       
        transform.LookAt(transform.position);
        _meshRenderer = gameObject.GetComponent<MeshRenderer>();
        _initalRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_active)
        {
            _liveTime -= Time.deltaTime;
            Rigidbody rb = GetComponent<Rigidbody>();
            Vector3 velo = rb.velocity;

            float mag = velo.magnitude +0.1f; // Speed
            Vector4 skew = new Vector4(5/mag, mag/100, 1f);
            _meshRenderer.material.SetVector("_Velocity", skew);
            transform.rotation = Quaternion.LookRotation(velo);
          
            if (_liveTime < 0)
            {
                Destroy(this.gameObject);
            }
        }
        
        if (_active && _sphere.transform.localScale.x < radius/2)
        {
            _sphere.transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * growthRate;
        }



    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.isStatic)
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = true;
            _isLocked = true;
            _active = true;
            gameObject.GetComponent<SphereCollider>().radius = radius;
            gameObject.GetComponent<SphereCollider>().isTrigger = true;
            _sphere.transform.position = transform.position;
            initialGravityChanger(collision);
            transform.rotation = Quaternion.FromToRotation(Vector3.up, _normal); 

            _meshRenderer.material.SetVector("_Velocity", new Vector4(1f, 0.2f, 2f));
            _navMeshSurface.BuildNavMesh();
            _navMeshLinks.Generate();
            _sphere.SetActive(true);

            
        }
    }

    public bool getIsLocked()
    {
        return _isLocked;
    }

    public void setIsLocked(bool locked)
    {
        _isLocked = locked;
    }

    public bool getActive()
    {
        return _active;
    }

    /**
     * initial gravity change after projectile is locked on the collision
     */
    private void initialGravityChanger(Collision collision)
    {
        _normal = collision.GetContact(0).normal;
        Collider[] co = Physics.OverlapSphere(_normal, radius, 0);
        foreach (Collider collider in co)
        {
            double direction = Vector3.Dot(_normal, (collider.transform.position - transform.position));
            if (direction > -2) changeGravity(collider, _normal * -9.81f);
        }

    }

    private void OnTriggerEnter(Collider other)
    {

        double direction = Vector3.Dot(_normal, (other.transform.position - transform.position));
        if (direction > -2) changeGravity(other, _normal * -9.81f);
        else changeGravity(other, _defaultGravity);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other);
        changeGravity(other, _defaultGravity);
    }

    private void OnDestroy()
    {
        Destroy(_sphere);
        Collider[] co = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider collider in co)
        {
            changeGravity(collider, _defaultGravity);
        }
    }

    /**
     * change gravity of Collider collider to the Vector3 gravity
     */
    private void changeGravity(Collider collider, Vector3 gravity)
    {
        /**
         * check if collider hast an gravityObject if not check if parent has one. 
         * Gravity of that GravityObject.
         */

        GravityObject go = collider.GetComponent<GravityObject>();
        if (!go) go = collider.GetComponentInParent<GravityObject>();
        if (go) go.SetLocalGravity(gravity);
        BaseAI ai = collider.GetComponent<BaseAI>();
        if (!ai) ai = collider.GetComponentInParent<BaseAI>();
        if (ai) ai.GravityChange();
    }

    private void OnTriggerStay(Collider other)
    {


        double direction = Vector3.Dot(_normal, (other.transform.position - transform.position));
        if (direction > -2) changeGravity(other, _normal * -9.81f);
        else changeGravity(other, _defaultGravity);
    }
}
