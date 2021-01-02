using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamScript : MonoBehaviour
{
    public GameObject beamPrefab;
    public GameObject firePoint;

    private GameObject spawnedBeam;
    void Start()
    {
        spawnedBeam = Instantiate(beamPrefab, firePoint.transform) as GameObject;


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            EnableBeam();
        }
        
        if (Input.GetMouseButton(1))
        {
            UpdateBeam();
        }
        if (Input.GetMouseButtonUp(1))
        {
            DisableBeam();
        }

    }

    void EnableBeam()
    {
        spawnedBeam.SetActive(true);
    }

    void UpdateBeam()
    {
        if (firePoint != null)
        {
            spawnedBeam.transform.position = firePoint.transform.position;
        }
    }

    void DisableBeam()
    {
        spawnedBeam.SetActive(false);
    }
}
