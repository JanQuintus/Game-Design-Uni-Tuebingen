using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
        Instance = this;
    }

    private void Start()
    {
        GameManager.CurrentScene = SceneManager.GetActiveScene().name;
    }

    public void LoadScene(string name, bool save = false)
    {
        GameManager.LastScene = GameManager.CurrentScene;
        GameManager.CurrentScene = name;
        if (save && SaveLoadSystem.Instance != null)
            SaveLoadSystem.Instance.Save();
        SceneManager.LoadScene("LoadingScreen", LoadSceneMode.Additive);
    }

}
