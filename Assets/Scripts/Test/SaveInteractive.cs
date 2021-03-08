using UnityEngine;

public class SaveInteractive : AInteractive
{
    public override void Interact(bool isRelease)
    {
        if (isRelease)
        {
            SaveLoadSystem.Instance.Save();
        }
    }
}
