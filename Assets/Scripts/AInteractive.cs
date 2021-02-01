using UnityEngine;

public abstract class AInteractive : MonoBehaviour
{
    [SerializeField] protected Material outlineMaterial;
    [SerializeField] protected AudioClip hoverClip;

    public abstract void Interact(bool isRelease);

    public virtual void Hover()
    {
        if(outlineMaterial != null)
            outlineMaterial.SetFloat("_Outline", 1f);
        if (hoverClip != null)
            SoundController.Instance.PlaySoundAtLocation(hoverClip, transform.position);
    }

    public virtual void Unhover()
    {
        if(outlineMaterial != null)
            outlineMaterial.SetFloat("_Outline", 0f);
    }

    public virtual string GetText() => "";
}