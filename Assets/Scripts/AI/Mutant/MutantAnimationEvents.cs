using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutantAnimationEvents : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClipBundle attackBundle;

    public void AttackPlayer()
    {
        if(Vector3.Distance(transform.position, PlayerController.Instance.transform.position) <= 3f)
        {
            PlayerController.Instance.GetComponentInParent<Health>().Damage(10);
            SoundController.Instance.PlaySFXPlayer(attackBundle.GetRandomClip(), Random.Range(0.5f, 1f), Random.Range(0.9f, 1.1f));
        }
    }
}
