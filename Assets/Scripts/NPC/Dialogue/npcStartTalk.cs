using UnityEngine;
using DG.Tweening;

public class npcStartTalk : AInteractive
{
    // Panel, bool for active dialogue box initialize
    public RectTransform Panel;

    public override void Interact(bool isRelease) // on Interact
    {
        if (!isRelease)
        {
            // if Panel isn't active already
            if (Panel.gameObject.activeSelf == false)
            {
                // scale down beforehand so it can get scaled up once visible
                Panel.DOScaleX(0.1f, 0f);
                Panel.DOScaleY(0.1f, 0f);

                // open Panel
                Panel.gameObject.SetActive(true);

                // scale up
                Panel.DOScaleX(1f, 0.2f);
                Panel.DOScaleY(1f, 0.2f);
            }

            // otherwise close it ( scale down and hide )
            else
            {
                Panel.DOScaleX(0.1f, 0.2f);
                Panel.DOScaleY(0.1f, 0.2f);
                Invoke("hidePanel", 0.2f);
            }
        }
    }


    private void hidePanel() // hidePanel, what does it do? :flushed:
    {
        Panel.gameObject.SetActive(false);
    }
}
