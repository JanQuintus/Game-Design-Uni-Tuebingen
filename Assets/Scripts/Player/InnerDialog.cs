using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerDialog : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public static InnerDialog Instance;

    private void Awake()
    {
        if(Instance != null) Debug.LogWarning("There are multiple InnerDialogs!", gameObject);
        Instance = this;
    }

    public void PlayDialog(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
}
