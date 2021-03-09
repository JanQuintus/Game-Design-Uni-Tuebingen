using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerDialogInteractable : AInteractive
{
    [SerializeField] private AudioClipBundle clipBundle;
    [SerializeField] private string hoverText;

    private float blockTime = 0;

    public override void Interact(bool isRelease)
    {
        if (isRelease || blockTime > 0f)
            return;

        AudioClip clip = clipBundle.GetRandomClip();
        InnerDialog.Instance.PlayDialog(clip);
        blockTime = clip.length;
    }

    private void Update()
    {
        if (blockTime > 0)
            blockTime -= Time.deltaTime;
    }

    public override string GetText()
    {
        return hoverText;
    }
}
