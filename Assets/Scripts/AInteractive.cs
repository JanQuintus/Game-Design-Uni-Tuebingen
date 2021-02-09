using UnityEngine;
using UnityEngine.Events;

public abstract class AInteractive : MonoBehaviour
{
    [SerializeField] protected Renderer outlineMaterial;
    private int _shader_active = Shader.PropertyToID("_Outline");
    [SerializeField] protected AudioClip hoverClip;

    public UnityEvent OnInteractionStart;
    public UnityEvent OnInteractionEnd;

    public virtual void Interact(bool isRelease)
    {
        OnInteractionStart.Invoke();
    }

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