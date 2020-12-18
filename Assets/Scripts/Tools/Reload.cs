using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reload : MonoBehaviour
{
    [SerializeField]
    private Transform checkpoint;

    [SerializeField]
    private float distance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        distance = (checkpoint.transform.position - transform.position).magnitude;
        if (distance <= 0)
        {
            gameObject.GetComponent<GravityGunTool>().Reload();
            gameObject.GetComponent<GravityLauncherTool>().Reload();
            gameObject.GetComponent<MassExchangerTool>().Reload();
            gameObject.GetComponent<TractorBeamTool>().Reload();
        }
    }
}
