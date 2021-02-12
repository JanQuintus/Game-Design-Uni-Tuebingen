using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private UIButton resetButton;

    private void Start()
    {
        if (!File.Exists(SaveLoadSystem.SavePath))
            resetButton.interactable = false;
    }

    public void Back()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void ResetSaveGame()
    {
        SaveLoadSystem.DeleteSaveFile();
    }
}
