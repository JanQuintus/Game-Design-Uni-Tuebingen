using UnityEngine;

public class WeightEnable : MonoBehaviour
{
    // yes
    public float massthresh;
    private Rigidbody _rb;

    private void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>(); // set rb on start so we only need to get component once
    }

    private void Update()
    {
        // if object weight > thresh
        if (_rb.mass > massthresh)
        {
            gameObject.GetComponent<GravityObject>().enabled = true; // enable gravobj
            Destroy(this); // remove this script
        }
    }
}
