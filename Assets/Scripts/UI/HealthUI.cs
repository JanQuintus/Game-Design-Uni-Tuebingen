using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Image fill;
    [SerializeField] private Health health;

    private void Awake()
    {
        health.OnDamaged.AddListener((amount) => UpdateUI());
        health.OnHealed.AddListener((amount) => UpdateUI());
    }

    private void UpdateUI()
    {
        fill.fillAmount = health.GetHealthPercentage();
    }
}
