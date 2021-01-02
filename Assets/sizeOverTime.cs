using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sizeOverTime : MonoBehaviour
{
    private float _amp = 0.01f;
   
    void Update()
    {
        
        Vector3 temp = transform.localScale;

        temp.x += Mathf.Sin(Time.time * 10.0f) * _amp;
        temp.y += Mathf.Cos(Time.time * 5.0f) * _amp;
        temp.z += Mathf.Cos(Time.time * 10.0f) * _amp;

        transform.localScale = temp;
        //float size = amp * Mathf.Sin(Time.time)+1;
        //transform.localScale = new Vector3(amp+2, amp+2, amp+2) *  ;
    }
}
