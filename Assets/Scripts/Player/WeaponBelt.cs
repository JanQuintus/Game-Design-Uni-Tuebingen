using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBelt : MonoBehaviour
{
    [SerializeField] private int startTool;
    [SerializeField] private ATool[] tools;
    [SerializeField] private int unlocked = 1;

    private void Awake()
    {
        foreach (ATool tool in tools)
            tool.gameObject.SetActive(false);
    }
    private void Start()
    {
        SetWeapon(startTool);
    }

    public void SetWeapon(int index)
    {
        if (index > unlocked)
            return;

        PlayerController.Instance.SetCurrentTool(tools[index]);
    }
}
