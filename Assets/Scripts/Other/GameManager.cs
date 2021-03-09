using UnityEngine;

public class GameManager
{
    public static System.Action OnPauseGame;
    public static System.Action OnUnpauseGame;
    public static System.Action OnGlobalProgressChanged;
    public static System.Action OnFoodChanged;
    public static System.Action OnDrinksChanged;

    private static int globalProgress = 0;
    private static int food = 0;
    private static int drinks = 0;

    public static int GlobalProgress { get { return globalProgress; } set { globalProgress = value; OnGlobalProgressChanged?.Invoke(); } }
    public static int Food { get { return food; } set { food = value; OnFoodChanged?.Invoke(); } }
    public static int Drinks { get { return drinks; } set { drinks = value; OnDrinksChanged?.Invoke(); } }

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
        PlayerPrefs.SetInt("food", Food);
        PlayerPrefs.SetInt("drinks", Drinks);
        PlayerPrefs.SetString("currentScene", CurrentScene != "MainMenu" ? CurrentScene : LastScene);
        PlayerPrefs.Save();
    }

    public static void Load()
    {
        GlobalProgress = PlayerPrefs.GetInt("globalProgress", 0);
        Food = PlayerPrefs.GetInt("food", 0);
        Drinks = PlayerPrefs.GetInt("drinks", 0);
        CurrentScene = PlayerPrefs.GetString("currentScene", "SLS_Test1");
    }
}
