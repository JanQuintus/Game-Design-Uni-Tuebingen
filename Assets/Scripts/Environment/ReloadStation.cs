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

    private float _currentTime = 0.0f;
    [SerializeField] private static float reloadDuration = 5.0f;
    [SerializeField] private int duration = 1;

    private bool _isReloading = false;

    public override void Interact(bool isRelease)
    {
        if (isRelease)
            return;

        if (!_isReloading)
        {
            _isReloading = true;
            _currentTime = Time.fixedTime;
            CloseWindow();
        }

        
    }

    private void Update()
    {
        if (_isReloading)
        {
            if (_currentTime != 0 && (Time.fixedTime - _currentTime) > reloadDuration)
            {
                OpenWindow();
                _currentTime = 0;
                PlayerController.Instance.GetCurrentTool()?.Reload();
                _isReloading = false;
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
}
