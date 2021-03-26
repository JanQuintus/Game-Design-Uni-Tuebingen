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
    [SerializeField] private DialogueScriptableObject dialogueScriptable0; // objects assigned per global progress
    [SerializeField] private DialogueScriptableObject dialogueScriptable1;
    [SerializeField] private DialogueScriptableObject dialogueScriptable2;
    [SerializeField] private DialogueScriptableObject dialogueScriptable3;
    [SerializeField] private DialogueScriptableObject dialogueScriptable4;
    [SerializeField] private DialogueScriptableObject dialogueScriptable5;
    [SerializeField] private DialogueScriptableObject dialogueScriptable6;
    [SerializeField] private DialogueScriptableObject dialogueScriptable7;

    private string _npcName;


    public override void Interact(bool isRelease) // on Interact
    {
        if (!isRelease)
        {
            _npcName = gameObject.name;

            switch(GameManager.GlobalProgress) // load dialogue depending on progress
            {
                case 0:
                    DialogueUI.Instance.StartDialogue(dialogueScriptable0, _npcName);
                    break;
                case 1:
                    DialogueUI.Instance.StartDialogue(dialogueScriptable1, _npcName);
                    break;
                case 2:
                    DialogueUI.Instance.StartDialogue(dialogueScriptable2, _npcName);
                    break;
                case 3:
                    DialogueUI.Instance.StartDialogue(dialogueScriptable3, _npcName);
                    break;
                case 4:
                    DialogueUI.Instance.StartDialogue(dialogueScriptable4, _npcName);
                    break;
                case 5:
                    DialogueUI.Instance.StartDialogue(dialogueScriptable5, _npcName);
                    break;
                case 6:
                    DialogueUI.Instance.StartDialogue(dialogueScriptable6, _npcName);
                    break;
                case 7:
                    DialogueUI.Instance.StartDialogue(dialogueScriptable7, _npcName);
                    break;
            }

            ai.SetCanMove(false);
            ai.SetLookAtTarget(PlayerController.Instance.transform.position);
            DialogueUI.Instance.OnDialogueEnded += OnDialogEnded; // fix dialogueui to new parser, see scriptobj
        }
    }

    private void OnDialogEnded()
    {
        ai.SetCanMove(true);
        DialogueUI.Instance.OnDialogueEnded -= OnDialogEnded; // also raise global progress if char is officer and global progress is at certain level
    }

    public override string GetText()
    {
        return "Talk";
    }
}


