using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turnOff_afterTime : MonoBehaviour
{
    public float time = 0.1f;

    void OnEnable()
    {
        StartCoroutine(Hide());
    }
    IEnumerator Hide()
    {
        yield return new WaitForSecondsRealtime(time);
        gameObject.SetActive(false);
    }
}

