using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutantAnimationEvents : MonoBehaviour
{
    public void AttackPlayer()
    {
        if(Vector3.Distance(transform.position, PlayerController.Instance.transform.position) <= 3f)
        {
            PlayerController.Instance.GetComponentInParent<Health>().Damage(10);
        }
    }
}
