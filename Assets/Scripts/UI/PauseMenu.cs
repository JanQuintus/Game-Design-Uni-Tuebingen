using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private MaskableGraphic bg;
    [SerializeField] private UIButton continueButton;
    [SerializeField] private MaskableGraphic[] graphics;

    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        GameManager.OnPauseGame += Show;
        GameManager.OnUnpauseGame += Hide;
        inputActions.UI.Cancel.performed += (context) =>
        {
            if (context.performed)
                Continue();
        };

        foreach (MaskableGraphic g in graphics)
        {
            Color c = g.color;
            c.a = 0f;
            g.color = c;
        }
        Color bgc = bg.color;
        bgc.a = 0f;
        bg.color = bgc;
    }

    private void OnDisable()
    {
        inputActions.UI.Disable();
    }

    private void OnDestroy()
    {
        GameManager.OnPauseGame -= Show;
        GameManager.OnUnpauseGame -= Hide;
    }

    public void Continue()
    {
        GameManager.UnpauseGame();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void MainMenu()
    {
        GameManager.UnpauseGame();
        SceneLoader.Instance.LoadScene("MainMenu");
    }

    private void Show()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        bg.DOFade(0.8f, 0.25f).SetUpdate(true);
        foreach (MaskableGraphic g in graphics)
            g.DOFade(1f, 0.25f).SetUpdate(true);
        inputActions.UI.Enable();
        continueButton.Select();
    }
    private void Hide()
    {
        bg.DOFade(0f, 0.25f).SetUpdate(true);
        foreach (MaskableGraphic g in graphics)
            g.DOFade(0f, 0.25f).SetUpdate(true);
        inputActions.UI.Disable();
    }
}
