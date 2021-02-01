using UnityEngine;

public abstract class AInteractive : MonoBehaviour
{
    [SerializeField] protected Renderer outlineMaterial;
    private int _shader_active = Shader.PropertyToID("_Outline");
    [SerializeField] protected AudioClip hoverClip;

    public abstract void Interact(bool isRelease);

    public virtual void Hover()
    {
        if(outlineMaterial != null)
            outlineMaterial.material.SetFloat(_shader_active, 1f);
        if (hoverClip != null)
            SoundController.Instance.PlaySoundAtLocation(hoverClip, transform.position);
    }

    public virtual void Unhover()
    {
        if(outlineMaterial != null)
            outlineMaterial.material.SetFloat(_shader_active, 0f);
    }

    public virtual string GetText() => "";
}