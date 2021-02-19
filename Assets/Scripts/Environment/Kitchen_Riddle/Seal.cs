using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Seal : MonoBehaviour
{

    [SerializeField] private bool isclosed = false;
    public void openSeal(float dir)
    {
        if (isclosed)
        {
            isclosed = false;
            transform.DORotate(transform.forward * dir, 2f, RotateMode.Fast);
        }

    }

    public void closeSeal(float dir)
    {
        if (!isclosed)
        {
            isclosed = true;
            transform.DORotate(transform.forward * dir, 2f, RotateMode.Fast);
        }
    }
}
