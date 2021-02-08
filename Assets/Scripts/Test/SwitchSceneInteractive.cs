using UnityEngine;

public class SwitchSceneInteractive : AInteractive
{
    [SerializeField] private string scene;

    public override void Interact(bool isRelease)
    {
        if (isRelease)
        {
            SceneLoader.Instance.LoadScene(scene);
        }
    }
}
