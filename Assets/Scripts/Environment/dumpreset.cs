using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dumpreset : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private GameObject resetPosition;

    private List<GameObject> savedObjects = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 0)
        {
            savedObjects.Add(other.gameObject);
            other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            other.gameObject.SetActive(false);
        }

    }

    public void Release()
    {
        foreach (GameObject element in savedObjects)
        {
            element.SetActive(true);
            element.transform.position = resetPosition.transform.position;
            element.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(0,5), Random.Range(0, 5), Random.Range(0, 5)), ForceMode.Impulse);
        }
        savedObjects.Clear();
    }
}
