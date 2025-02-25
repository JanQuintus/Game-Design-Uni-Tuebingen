﻿using UnityEngine;
using UnityEngine.Events;

public abstract class AInteractive : MonoBehaviour
{
    [SerializeField] protected Outline outline;
    [SerializeField] protected float outlineWidth;
    private int _shader_active = Shader.PropertyToID("_Outline");

    [SerializeField] protected AudioClip hoverClip;

    public abstract void Interact(bool isRelease);

    public virtual void Hover()
    {
        if(outline != null) {
            //outlineMaterial.material.SetFloat(_shader_active, 1f);
            //var outline = gameObject.GetComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineAll;
            outline.OutlineWidth = outlineWidth;
        }

        if (hoverClip != null)
            SoundController.Instance.PlaySoundAtLocation(hoverClip, transform.position);
    }

    public virtual void Unhover()
    {
        if (outline != null)
        {
            //outlineMaterial.material.SetFloat(_shader_active, 0f);
            //var outline = gameObject.GetComponent<Outline>();
            outline.OutlineWidth = 0f;
            outline.OutlineMode = Outline.Mode.OutlineHidden;
            

        }

    }

    public virtual string GetText() => "";
}