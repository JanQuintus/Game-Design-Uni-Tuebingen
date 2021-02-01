using UnityEngine;

public abstract class AInteractive : MonoBehaviour
{
    [SerializeField] protected Renderer outlineMaterial;
    private int _shader_active = Shader.PropertyToID("_Outline");

    public abstract void Interact(bool isRelease);

    public virtual void ShowOutline()
    {
        if(outlineMaterial != null)
            outlineMaterial.material.SetFloat(_shader_active, 1f);
    }

    public virtual void HideOutline()
    {
        if(outlineMaterial != null)
            outlineMaterial.material.SetFloat(_shader_active, 0f);
    }

    public virtual string GetText() => "";
}