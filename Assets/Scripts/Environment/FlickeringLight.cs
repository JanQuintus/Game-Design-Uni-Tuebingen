using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlickeringLight : MonoBehaviour
{
    [SerializeField] private AudioClip blinkOnClip;
    [SerializeField] private AudioClip blinkOffClip;

    [SerializeField] [Range(0, 10)] private float minOnTime = 0.5f;
    [SerializeField] [Range(0, 10)] private float maxOnTime = 3f;
    [SerializeField] [Range(0, 10)] private float minOffTime = 0.1f;
    [SerializeField] [Range(0, 10)] private float maxOffTime = 0.5f;

    private Light _light;
    private float _lightIntensity;
    private float _t;

    private void Awake()
    {
        _light = GetComponent<Light>();
        _lightIntensity = _light.intensity;
        _t = Random.Range(minOnTime, maxOnTime);
    }

    private void Update()
    {
        _t -= Time.deltaTime;
        if(_t <= 0)
        {
            if(_light.intensity == 0)
            {
                _t = Random.Range(minOnTime, maxOnTime);
                _light.intensity = _lightIntensity;
                SoundController.Instance.PlaySoundAtLocation(blinkOnClip, transform.position, Random.Range(0.5f, 1f));
            }
            else
            {
                _t = Random.Range(minOffTime, maxOffTime);
                _light.intensity = 0;
                SoundController.Instance.PlaySoundAtLocation(blinkOffClip, transform.position, Random.Range(0.5f, 1f));
            }
        }
    }
}
