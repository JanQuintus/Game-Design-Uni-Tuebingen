using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankFill : MonoBehaviour
{
    public ATool gravityLauncher;
    private Material mat;
    // Start is called before the first frame update

    private void Awake()
    {
        mat = GetComponent<Renderer>().material;
        mat.SetFloat("liquid", gravityLauncher.getFillPercentage());
    }


    // Update is called once per frame
    void Update()
    {
        mat.SetFloat("liquid", gravityLauncher.getFillPercentage());
    }
}
