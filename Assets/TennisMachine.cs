using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TennisMachine : MonoBehaviour
{
    [SerializeField] private Transform shootPoint;
    [SerializeField] private float shootForce = 20f;
    [SerializeField] private float shootInterval = 1f;

    private float _time;

    private void Awake()
    {
        _time = shootInterval;
    }
    void Start()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {

        if(_time == 0 && other.gameObject.tag.Equals("TennisBall") && other.GetComponent<GravityObject>().goEnabled)
        {
            _time = shootInterval;
            other.transform.position = shootPoint.position;
            other.GetComponent<Rigidbody>().velocity = shootPoint.forward * shootForce;
        }
    }

    void Update()
    {
        if (_time > 0)
        {
            _time -= Time.deltaTime;

            if (_time <= 0) {
                _time = 0;
            }

        }
    }
}
