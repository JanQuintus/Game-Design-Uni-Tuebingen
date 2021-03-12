using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RandomCameraShake : MonoBehaviour
{
    [SerializeField] [Range(0.1f, 10)] private float minSpeed = 2f;
    [SerializeField] [Range(0.1f, 10)] private float maxSpeed = 5f;
    [SerializeField] [Range(0f, 10)] private float minStrengthPos = .1f;
    [SerializeField] [Range(0f, 10)] private float maxStrengthPos = .4f;
    [SerializeField] [Range(0f, 10)] private float minStrengthRot = .1f;
    [SerializeField] [Range(0f, 10)] private float maxStrengthRot = .4f;

    private float _t;

    private void Awake()
    {
        _t = Random.Range(minSpeed, maxSpeed);
        transform.DOShakePosition(_t, Random.Range(minStrengthPos, maxStrengthPos), vibrato: 1, randomness: 10, fadeOut: false).SetEase(Ease.InOutSine);
        transform.DOShakeRotation(_t, Random.Range(minStrengthRot, maxStrengthRot), vibrato: 1, randomness: 10, fadeOut: false).SetEase(Ease.InOutSine);
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }

    private void Update()
    {
        _t -= Time.deltaTime;
        if(_t <= 0)
        {
            _t = Random.Range(minSpeed, maxSpeed);
            transform.DOShakePosition(_t, Random.Range(minStrengthPos, maxStrengthPos), vibrato: 1, randomness: 10, fadeOut: false).SetEase(Ease.InOutSine);
            transform.DOShakeRotation(_t, Random.Range(minStrengthRot, maxStrengthRot), vibrato: 1, randomness: 10, fadeOut: false).SetEase(Ease.InOutSine);
        }
    }
}
