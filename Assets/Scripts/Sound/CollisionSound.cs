using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSound : MonoBehaviour
{
    [SerializeField] private AudioClip collisionClip;

    private void OnCollisionEnter(Collision collision)
    {
        float volume = Mathf.Clamp01(collision.relativeVelocity.magnitude / 100f);
        float pitch = Mathf.Clamp(collision.relativeVelocity.magnitude / 50f, 0, 3f);
        SoundController.Instance.PlaySoundAtLocation(collisionClip, collision.GetContact(0).point, volume, pitch);
    }

    public AudioClip GetCollisionClip() => collisionClip;
}
