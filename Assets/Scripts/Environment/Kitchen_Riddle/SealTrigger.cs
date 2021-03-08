using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SealTrigger : MonoBehaviour
{
    [SerializeField] Seal seal;


    private void OnTriggerEnter(Collider other)
    {
        seal.closeSeal(-90f);
    }
}
