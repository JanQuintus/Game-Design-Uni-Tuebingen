using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankFill : MonoBehaviour
{
    public ATool gravityLauncher;
    private Material mat;

    Vector3 lastPos;
    Vector3 velocity;
    Vector3 lastRot;
    Vector3 angularVelocity;
    public float MaxWiggle = 0.03f;
    public float WiggleSpeed = 1f;
    public float Recovery = 1f;
    float wiggleAmountX;
    float wiggleAmountZ;
    float wiggleAmountToAddX;
    float wiggleAmountToAddZ;
    float pulse;
    float time = 0.5f;
    // Start is called before the first frame update

    private void Awake()
    {
        mat = GetComponent<Renderer>().material;
        mat.SetFloat("liquid", gravityLauncher.GetFillPercentage());
    }



    private void Update()
    {
        mat.SetFloat("liquid", gravityLauncher.GetFillPercentage());
        time += Time.deltaTime;
        // decrease Wiggle over time
        wiggleAmountToAddX = Mathf.Lerp(wiggleAmountToAddX, 0, Time.deltaTime * (Recovery));
        wiggleAmountToAddZ = Mathf.Lerp(wiggleAmountToAddZ, 0, Time.deltaTime * (Recovery));

        // make a sine wave of the decreasing Wiggle
        pulse = 2 * Mathf.PI * WiggleSpeed;
        wiggleAmountX = wiggleAmountToAddX * Mathf.Sin(pulse * time);
        wiggleAmountZ = wiggleAmountToAddZ * Mathf.Sin(pulse * time);

        // send it to the shader
        mat.SetFloat("_WiggleX", wiggleAmountX);
        mat.SetFloat("_WiggleZ", wiggleAmountZ);

        // velocity
        velocity = (lastPos - transform.position) / Time.deltaTime;
        angularVelocity = transform.rotation.eulerAngles - lastRot;


        // add clamped velocity to Wiggle
        wiggleAmountToAddX += Mathf.Clamp((velocity.x + (angularVelocity.z * 0.2f)) * MaxWiggle, -MaxWiggle, MaxWiggle);
        wiggleAmountToAddZ += Mathf.Clamp((velocity.z + (angularVelocity.x * 0.2f)) * MaxWiggle, -MaxWiggle, MaxWiggle);

        // keep last position
        lastPos = transform.position;
        lastRot = transform.rotation.eulerAngles;
    }



}

