using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private UIButton playButton;
    [SerializeField] private UIButton continueButton;

    private void Awake()
    {
        playButton.Select();
        continueButton.interactable = false;    
    }

    public void Continue()
    {

    }

    public void Play()
    {

    }

    public void Settings()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }
}
