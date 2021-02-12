using UnityEngine;

public class SaveInteractive : AInteractive
{
    public override void Interact(bool isRelease)
    {
        base.Interact(isRelease);
        if (isRelease)
        {
            SaveLoadSystem.Instance.Save();
        }
    }
}
