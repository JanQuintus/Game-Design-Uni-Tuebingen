using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] private AudioSourcePool pool;

    [Header("Player Audio")]
    [SerializeField] private Transform followTrans;
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioSource ambientAudioSource;

    public static SoundController Instance;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        musicAudioSource.transform.position = followTrans.position;
        sfxAudioSource.transform.position = followTrans.position;
        ambientAudioSource.transform.position = followTrans.position;
    }

    public void PlaySFXPlayer(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        sfxAudioSource.Stop();
        sfxAudioSource.clip = clip;
        sfxAudioSource.volume = volume;
        sfxAudioSource.pitch = pitch;
        sfxAudioSource.Play();
    }

    public void PlaySoundAtLocation(AudioClip clip, Vector3 position, float volume = 1f, float range = 50f, float pitch = 1f)
    {
        AudioSource audioSource = pool.GetAudioSource(position, volume, range, pitch);
        audioSource.clip = clip;
        audioSource.Play();
        StartCoroutine(WatchAudioSourceAndFree(audioSource));
    }

    private IEnumerator WatchAudioSourceAndFree(AudioSource audioSource)
    {
        yield return new WaitUntil(() => !audioSource.isPlaying);
        pool.FreeAudioSource(audioSource);
    }
}
