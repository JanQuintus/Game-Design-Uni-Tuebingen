using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerTeleporter : MonoBehaviour
{
    private void Start()
    {
        foreach(PlayerTeleportPoint ptp in FindObjectsOfType<PlayerTeleportPoint>())
        {
            if(ptp.LastScene == GameManager.LastScene)
            {
                PlayerController.Instance.transform.position = ptp.transform.position;
                PlayerController.Instance.transform.rotation = ptp.transform.rotation;
                break;
            }
        }   
    }
}
