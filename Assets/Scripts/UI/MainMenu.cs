using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private UIButton playButton;
    [SerializeField] private UIButton continueButton;
    [SerializeField] private Texture2D cursor;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
    }

    private void Start()
    {
        if (File.Exists(SaveLoadSystem.SavePath))
        {
            Destroy(playButton.gameObject);
            continueButton.Select();
        }
        else
        {
            Destroy(continueButton.gameObject);
            playButton.Select();
        }
    }

    public void Continue()
    {
        SceneLoader.Instance.LoadScene(PlayerPrefs.GetString("currentScene", "SLS_Test1"));
    }

    public void Play()
    {
        SaveLoadSystem.DeleteSaveFile();
        SceneLoader.Instance.LoadScene("IntroCinematic");
    }

    public void Settings()
    {
        SceneManager.LoadSceneAsync("SettingsMenu", LoadSceneMode.Additive);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
