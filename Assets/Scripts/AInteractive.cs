using UnityEngine;

public abstract class AInteractive : MonoBehaviour
{
    [SerializeField] protected Material outlineMaterial;

    public abstract void Interact(bool isRelease);

    public virtual void ShowOutline()
    {
        if(outlineMaterial != null)
            outlineMaterial.SetFloat("_Outline", 1f);
    }

    public virtual void HideOutline()
    {
        if(outlineMaterial != null)
            outlineMaterial.SetFloat("_Outline", 0f);
    }

    public virtual string GetText() => "";
}