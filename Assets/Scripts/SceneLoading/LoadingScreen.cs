using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Image blend;
    [SerializeField] private GameObject loadingScreenCamera;

    private void Start()
    {
        blend.DOFade(1f, 1.5f).onComplete += () =>
        {
            AsyncOperation unload = SceneManager.UnloadSceneAsync(GameManager.LastScene);
            unload.completed += (AsyncOperation unloadOperation) =>
            {
                loadingScreenCamera.SetActive(true);
                Scene loadingScreenScene = SceneManager.GetSceneByName("LoadingScreen");
                AsyncOperation load = SceneManager.LoadSceneAsync(GameManager.CurrentScene, LoadSceneMode.Additive);
                load.completed += (AsyncOperation asyncOperation) =>
                {
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName(GameManager.CurrentScene));
                    loadingScreenCamera.SetActive(false);
                    blend.DOFade(0f, 1.5f).onComplete += () => SceneManager.UnloadSceneAsync(loadingScreenScene);
                };
            };
        };
    }
}
