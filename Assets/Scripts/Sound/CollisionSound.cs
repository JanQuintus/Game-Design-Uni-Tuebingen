using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSound : MonoBehaviour
{
    [SerializeField] private AudioClip collisionClip;

    private float _tNextSound = 0f;

    private void Update()
    {
        if (_tNextSound >= 0)
        {
            _tNextSound -= Time.deltaTime;
            if (_tNextSound <= 0)
                _tNextSound = 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_tNextSound > 0f)
            return;
        if (collision.relativeVelocity.magnitude <= 10) return;
        _tNextSound = 0.5f;
        float volume = Mathf.Clamp01(collision.relativeVelocity.magnitude / 100f);
        float pitch = Mathf.Clamp(collision.relativeVelocity.magnitude / 50f, 0, 3f);
        SoundController.Instance.PlaySoundAtLocation(collisionClip, collision.GetContact(0).point, volume, pitch);
    }

    public AudioClip GetCollisionClip() => collisionClip;
}