using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ReloadStation : AInteractive, ISaveable
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

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip windowOpenClip;
    [SerializeField] private AudioClip windowCloseClip;

    private float _currentTime = 0.0f;
    [SerializeField] private static float reloadDuration = 5.0f;
    [SerializeField] private float duration = 1;
    [SerializeField] private float lightintensity = 0.04f;

    private bool _isReloading = false;
    private ATool _currentTool;

    public override void Interact(bool isRelease)
    {
        base.Interact(isRelease);
        if (isRelease)
            return;

        if (!_isReloading && PlayerController.Instance.GetCurrentTool() != null && _currentTool == null)
        {
            if(_currentTool == null)
            {
                _isReloading = true;
                _currentTime = Time.fixedTime;
                _currentTool = PlayerController.Instance.GetCurrentTool();
                showAndDisableTool(true);
                PlayerController.Instance.GetToolBelt().BlockTool(PlayerController.Instance.GetCurrentTool());

                Kühlschranklicht.DOIntensity(0.04f, timeToLightTheCabin);

                CloseWindow();
                audioSource.PlayOneShot(windowCloseClip);
                audioSource.Play();
            }
        }
        if(!_isReloading && _currentTool != null)
        {
            showAndDisableTool(false);
            PlayerController.Instance.GetToolBelt().UnblockTool(_currentTool);
            _currentTool = null;
        }
        OnInteractionEnd.Invoke();
    }

    private void Update()
    {
        if (_isReloading)
        {
            if (_currentTime != 0 && (Time.fixedTime - _currentTime) > reloadDuration) // reload finished
            {
                OpenWindow();
                _currentTime = 0;
               _currentTool.Reload();
                _isReloading = false;
                Kühlschranklicht.DOIntensity(0f, timeToLightTheCabin);
                audioSource.Stop();
                audioSource.PlayOneShot(windowOpenClip);

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

    private void showAndDisableTool(bool isActive)
    {
        if (_currentTool is GravityGunTool)
        {
            GravityGun.SetActive(isActive);
        }
        else if (_currentTool is TractorBeamTool)
        {
            TractorBeam.SetActive(isActive);
        }
        else if (_currentTool is MassExchangerTool)
        {
            MassExchanger.SetActive(isActive);
        }
        else if(_currentTool is GravityLauncherTool)
        {
            GravityLauncher.SetActive(isActive);
        }
    }

    public override string GetText()
    {
        if (PlayerController.Instance.GetCurrentTool() != null && !_isReloading && _currentTool == null)
            return "Reload <b>" + PlayerController.Instance.GetCurrentTool().ToolName + "</b>";

        if (_isReloading)
            return "";

        if (!_isReloading && _currentTool != null)
            return "Take <b>" + _currentTool.ToolName + "</b>";

        return "";
    }

    public object CaptureState()
    {
        return new SaveState
        {
            IsReloading = _isReloading,
            CurrentTool = _currentTool != null ? _currentTool.GetType().ToString() : "null"
        };
    }

    public void RestoreState(object state)
    {
        SaveState saveState = (SaveState)state;

        _isReloading = saveState.IsReloading;
        if (_isReloading)
        {
            CloseWindow();
            _currentTime = Time.fixedTime;
            audioSource.Play();
            Kühlschranklicht.DOIntensity(0.04f, timeToLightTheCabin);

            _currentTool = PlayerController.Instance.GetToolBelt().GetToolByClassName(saveState.CurrentTool);
            showAndDisableTool(true);
        }
    }

    [System.Serializable]
    private struct SaveState
    {
        public bool IsReloading;
        public string CurrentTool;
    }
}
