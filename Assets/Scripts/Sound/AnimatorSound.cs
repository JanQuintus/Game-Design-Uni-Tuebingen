using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public void PlayAudioClip(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
