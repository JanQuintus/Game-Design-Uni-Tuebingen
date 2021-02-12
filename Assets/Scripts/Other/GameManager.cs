using UnityEngine;

public class GameManager
{
    public static System.Action OnPauseGame;
    public static System.Action OnUnpauseGame;

    public static int GlobalProgress = 0;
    public static string LastScene;
    public static string CurrentScene;

    public static void PauseGame()
    {
        Time.timeScale = 0;
        OnPauseGame?.Invoke();
    }

    public static void UnpauseGame()
    {
        Time.timeScale = 1;
        OnUnpauseGame?.Invoke();
    }

    public static void Save()
    {
        PlayerPrefs.SetInt("globalProgress", GlobalProgress);
        PlayerPrefs.SetString("currentScene", CurrentScene != "MainMenu" ? CurrentScene : LastScene);
        PlayerPrefs.Save();
    }

    public static void Load()
    {
        GlobalProgress = PlayerPrefs.GetInt("globalProgress", 0);
        CurrentScene = PlayerPrefs.GetString("currentScene", "SLS_Test1");
    }
}
