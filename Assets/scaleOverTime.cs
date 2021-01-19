using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scaleOverTime : MonoBehaviour
{
    //private GameObject _self;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //ParticleSystem ps = GetComponent<ParticleSystem>();
        //ps.shape.scale.x = 3 * Mathf.Sin(Time.time) + 1;
        //Transform obj = GetComponent<Transform>();

        float sizeChanger =  1+ 0.3f * Mathf.Sin(-Time.deltaTime *5) + 0.3f;
        transform.localScale = new Vector3(sizeChanger, 1, sizeChanger);



    }
}
