using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
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
