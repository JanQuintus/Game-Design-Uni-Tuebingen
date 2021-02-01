using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourcePool : MonoBehaviour
{
    [SerializeField] private AudioSource reference;
    [SerializeField] private int poolSize = 20;

    private Stack<AudioSource> _audioSources = new Stack<AudioSource>();
    private AudioSource _fallback;

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
            _audioSources.Push(SpawnAudioSource(i));
        _fallback = SpawnAudioSource(999);
        _fallback.priority = 0;
        _fallback.maxDistance = 0;
        _fallback.minDistance = 0;
        _fallback.mute = true;
        _fallback.spatialBlend = 0;
    }

    public AudioSource GetAudioSource(Vector3 position, float volume = 1f, float range = 50f, float pitch = 1f)
    {
        if(_audioSources.Count == 0)
            return _fallback;

        AudioSource audioSource = _audioSources.Pop();

        audioSource.enabled = true;
        audioSource.volume = volume;
        audioSource.transform.position = position;
        audioSource.maxDistance = range;
        audioSource.pitch = pitch;

        return audioSource;
    }

    public void FreeAudioSource(AudioSource audioSource)
    {
        if (audioSource == _fallback)
            return;

        audioSource.enabled = false;
        audioSource.transform.position = Vector3.zero;

        _audioSources.Push(audioSource);
    }

    private AudioSource SpawnAudioSource(int index)
    {
        AudioSource audioSource = Instantiate(reference, transform);
        audioSource.enabled = false;
        audioSource.transform.position = Vector3.zero;
        audioSource.name = "3DAudioSource_" + index;
        return audioSource;
    }
}
