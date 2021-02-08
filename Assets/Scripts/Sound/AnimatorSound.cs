using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;


    public void PlayPlayer(AudioClip clip) => SoundController.Instance.PlaySFXPlayer(clip);
    public void PlayAudioSource(AudioClip clip) => audioSource.PlayOneShot(clip);
}
