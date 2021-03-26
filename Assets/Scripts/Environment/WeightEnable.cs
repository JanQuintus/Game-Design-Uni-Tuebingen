using UnityEngine;

public class WeightEnable : MonoBehaviour,ISaveable
{
    // yes
    public float massthresh;
    private Rigidbody _rb;
    private bool wasTriggered = false;

    private void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>(); // set rb on start so we only need to get component once
    }

    private void Update()
    {
        if (wasTriggered) return;

        // if object weight > thresh
        if (_rb.mass > massthresh)
        {
            gameObject.GetComponent<GravityObject>().enabled = true; // enable gravobj
            wasTriggered = true;
        }
    }

    public object CaptureState()
    {
        return wasTriggered;
    }

    public void RestoreState(object state)
    {
        wasTriggered = (bool)state;
        if (wasTriggered)
        {
            gameObject.GetComponent<GravityObject>().enabled = true;
        }
    }
}
