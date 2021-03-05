using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class NPCDialogue : AInteractive
{
    // Panel for active dialogue box initialize
    [SerializeField] private QuestNPCAI ai;
    [SerializeField] private DialogueScriptableObject scriptObjText;

    public override void Interact(bool isRelease) // on Interact
    {
        if (!isRelease)
        {
            DialogueUI.Instance.StartDialogue(scriptObjText);
            ai.SetCanMove(false);
            ai.SetLookAtTarget(PlayerController.Instance.transform.position);
            DialogueUI.Instance.OnDialogueEnded += OnDialogEnded;
        }
    }

    private void OnDialogEnded()
    {
        ai.SetCanMove(true);
        DialogueUI.Instance.OnDialogueEnded -= OnDialogEnded;
    }

    public override string GetText()
    {
        return "Talk";
    }
}


