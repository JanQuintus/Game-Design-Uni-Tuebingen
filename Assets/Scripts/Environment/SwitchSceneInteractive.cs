using UnityEngine;

public class SwitchSceneInteractive : AInteractive
{
    [SerializeField] [Scene] private string scene;
    [SerializeField] private string interactText;
    [SerializeField] private bool save = true;

    public override void Interact(bool isRelease)
    {
        if (isRelease)
        {
            SceneLoader.Instance.LoadScene(scene, save);
        }
    }

    public override string GetText()
    {
        return interactText;
    }
}
