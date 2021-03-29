using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Image blend;
    [SerializeField] private Image progress;
    [SerializeField] private Image image;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private MaskableGraphic[] blendInOut;
    [SerializeField] private GameObject loadingScreenCamera;

    private AsyncOperation load;

    private void Awake()
    {
        blend.color = new Color(blend.color.r, blend.color.g, blend.color.b, 0);
        progress.color = new Color(progress.color.r, progress.color.g, progress.color.b, 0);
        foreach (MaskableGraphic g in blendInOut) g.color = new Color(g.color.r, g.color.g, g.color.b, 0);
        image.sprite = sprites[Random.Range(0, sprites.Length)];
    }

    private void Start()
    {
        progress.DOFade(1f, 0.5f);
        foreach (MaskableGraphic g in blendInOut) g.DOFade(1f, 0.5f);

        ShowNextImage();

        blend.DOFade(1f, 0.5f).onComplete += () =>
        {
            if (SceneManager.GetSceneByName("MainMenu").isLoaded)
            {
                AsyncOperation unload = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("MainMenu"));
                unload.completed += (AsyncOperation unloadOperation) =>
                {
                    unloadAndLoad();
                };
            }
            else
            {
                unloadAndLoad();
            }
            
        };
    }

    private void unloadAndLoad()
    {
        if (SceneManager.GetSceneByName(GameManager.LastScene).isLoaded)
        {
            AsyncOperation unload = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(GameManager.LastScene));
            unload.completed += (AsyncOperation unloadOperation) =>
            {
                if (SceneManager.GetSceneByName(GameManager.CurrentScene).isLoaded)
                {
                    AsyncOperation unload2 = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(GameManager.CurrentScene));
                    unload2.completed += (AsyncOperation unloadOperation2) => loadNextScene();
                }
                else
                    loadNextScene();
            };
        }
        else
        {
            if (SceneManager.GetSceneByName(GameManager.CurrentScene).isLoaded)
            {
                AsyncOperation unload2 = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(GameManager.CurrentScene));
                unload2.completed += (AsyncOperation unloadOperation2) => loadNextScene();
            }
            else
                loadNextScene();
        }
    }

    private void loadNextScene()
    {
        loadingScreenCamera.SetActive(true);
        Scene loadingScreenScene = SceneManager.GetSceneByName("LoadingScreen");
        load = SceneManager.LoadSceneAsync(GameManager.CurrentScene, LoadSceneMode.Additive);
        load.completed += (AsyncOperation asyncOperation) =>
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(GameManager.CurrentScene));
            loadingScreenCamera.SetActive(false);
            if (image.rectTransform) image.rectTransform.DOKill();
            if (image) image.DOKill();
            if (progress) progress.DOFade(0f, 0.3f);
            foreach (MaskableGraphic g in blendInOut) if (g) g.DOFade(0f, 0.3f);
            if (blend)
                blend.DOFade(0f, 0.5f).onComplete += () => SceneManager.UnloadSceneAsync(loadingScreenScene);
            else
                SceneManager.UnloadSceneAsync(loadingScreenScene);
        };
    }

    private void ShowNextImage()
    {
        if (!image) return;
        image.DOFade(0f, 0.25f).onComplete += () =>
        {
            if(image)image.sprite = sprites[Random.Range(0, sprites.Length)];
            if(image)image.rectTransform.localScale = Vector3.one;
            if(image)image.DOFade(1f, 0.25f);
            if(image)image.rectTransform.DOScale(1.15f, 10f).onComplete += () => ShowNextImage();
        };
    }

    private void Update()
    {
        if(load != null)
            progress.fillAmount = load.progress;
    }
}
