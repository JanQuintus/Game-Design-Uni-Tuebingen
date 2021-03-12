using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UIButton : Selectable, ISubmitHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    public UnityEvent OnClick;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private AudioClip hoverClip;

    private bool _hover = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!interactable) return;
        audioSource.PlayOneShot(clickClip);
        OnClick.Invoke();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if (!interactable) return;
        if (currentSelectionState != SelectionState.Selected) return;
        audioSource.PlayOneShot(clickClip);
        DoStateTransition(SelectionState.Pressed, false);
        StartCoroutine(UnPress());
        OnClick.Invoke();
    }

    private IEnumerator UnPress()
    {
        yield return new WaitForSeconds(0.1f);
        if (currentSelectionState == SelectionState.Selected)
            DoStateTransition(SelectionState.Selected, false);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (!interactable) return;
        if (currentSelectionState == SelectionState.Selected) return;
        _hover = true;
        audioSource.PlayOneShot(hoverClip);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (!interactable) return;
        _hover = false;
    }

    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);
        if (!interactable) return;
        if (_hover) return;
        audioSource.PlayOneShot(hoverClip);
    }

}
