using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WallOfDeath : MonoBehaviour
{

    [SerializeField] private GameObject endPos;
    [SerializeField] private int duration = 500;

    void Start()
    {
        gameObject.transform.DOMove(endPos.transform.position, duration);
    }

    private void OnTriggerEnter(Collider col)
    {
        Health health = Utils.FindHealth(col);
        if (health != null)
        {
            if (health.GetHealth() > 0)
            {
                health.Damage(health.GetHealth());
            }
        }
    }


}
