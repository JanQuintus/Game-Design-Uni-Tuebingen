using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ReloadStation : AInteractive
{

    [SerializeField] GameObject GravityLauncher;
    [SerializeField] GameObject GravityGun;
    [SerializeField] GameObject MassExchanger;
    [SerializeField] GameObject TractorBeam;
    [SerializeField] GameObject Window;
    [SerializeField] GameObject EndPos;
    [SerializeField] GameObject StartPos;
    [SerializeField] Light Kühlschranklicht;
    [SerializeField] float timeToLightTheCabin = 0.1f;

    private float _currentTime = 0.0f;
    [SerializeField] private static float reloadDuration = 5.0f;
    [SerializeField] private int duration = 1;
    [SerializeField] private float lightintensity = 0.04f;

    private bool _isReloading = false;
    private string _currentTool;

    public override void Interact(bool isRelease)
    {
        if (isRelease)
            return;

        if (!_isReloading)
        {
            _isReloading = true;
            _currentTime = Time.fixedTime;
            _currentTool = PlayerController.Instance.GetCurrentTool().name;
            PlayerController.Instance.GetCurrentTool()?.Reload();

            showAndDisableTool(_currentTool, true);
            Kühlschranklicht.DOIntensity(lightintensity, timeToLightTheCabin);

            CloseWindow();
        }

        
    }

    private void Update()
    {
        if (_isReloading)
        {
            if (_currentTime != 0 && (Time.fixedTime - _currentTime) > reloadDuration) // reload finished
            {
                OpenWindow();
                _currentTime = 0;
                _isReloading = false;
                showAndDisableTool(_currentTool, false);
                Kühlschranklicht.DOIntensity(0f, timeToLightTheCabin);
            }
        }

    }

    private void OpenWindow()
    {
        Window.transform.DOMove(EndPos.transform.position, duration);
    }

    private void CloseWindow()
    {
        Window.transform.DOMove(StartPos.transform.position, duration);
    }

    private void showAndDisableTool(string currentTool, bool isActive)
    {
        if (currentTool == "GravityGun")
        {
            GravityGun.SetActive(isActive);
        }
        else if (currentTool == "TractorBeam")
        {
            TractorBeam.SetActive(isActive);
        }
        else if (currentTool == "MassExchanger")
        {
            MassExchanger.SetActive(isActive);
        }
        else if (currentTool == "GravityLauncher")
        {
            GravityLauncher.SetActive(isActive);
        }
    }
}
