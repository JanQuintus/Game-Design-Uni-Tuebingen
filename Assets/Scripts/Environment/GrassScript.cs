using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassScript : MonoBehaviour
{
    public Vector3 position;
    public Material material;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("updateMaterial");
    }

    // Update is called once per frame
    IEnumerator updateMaterial()
    {
        while (true)
        {
            position = PlayerController.Instance.transform.position;

            material.SetVector("playerPosition", position);
            yield return null;

        }

    }
}
