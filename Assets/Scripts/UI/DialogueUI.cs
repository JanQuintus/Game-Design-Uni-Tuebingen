using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public Action OnDialogueEnded;

    public static DialogueUI Instance;

    [SerializeField] private MaskableGraphic bg;
    [SerializeField] private TMP_Text choice1; // unused, can cull in later update
    [SerializeField] private TMP_Text choice2;
    [SerializeField] private TMP_Text choice3;
    [SerializeField] private TMP_Text choice4;
    [SerializeField] private TMP_Text dialogueText; // ( obv this one isn't unused lol )

    private PlayerInputActions _inputActions;

    private int _localDialogueProgress = 0;

    private AudioClip dialogueClip;

    private string _npcText;
    private string _npcName;

    private bool stopChat;

    private DialogueScriptableObject usedDialogueScriptable; // cuz we call thing in awake and can't give parameters hence

    public bool IsOpen() => bg.gameObject.activeSelf;

    private void Awake()
    {
        stopChat = false;

        if (Instance != null) // clear
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        bg.gameObject.SetActive(false); // turn ui things off
        choice1.gameObject.SetActive(false); // can cull later, also remove from canvas
        choice2.gameObject.SetActive(false);
        choice3.gameObject.SetActive(false);
        choice4.gameObject.SetActive(false);
        dialogueText.gameObject.SetActive(false);
        _inputActions = new PlayerInputActions(); // interaction decl
        _inputActions.UI.Interact.performed += (InputAction.CallbackContext context) => // define context as interact
        {
            if (context.performed) // on interact
            {
                localTextProcessing(); // parse string based on conversation path progress ( see below, culls per press )
            }
        };
        _inputActions.UI.Cancel.performed += (InputAction.CallbackContext context) =>
        {
            if (context.performed && bg.gameObject.activeSelf)
            {
                stopChat = true;
                hidePanel(false);
                Reset();
            }
        };

        
    }

    public void StartDialogue(DialogueScriptableObject dialogueScriptable, string npcName) // npcdialogue starts this, has stop condition and sets text depending on local prog
    { // also enables dialogueText gameObj, enters "chat mode" and starts localTextProcessing()
        usedDialogueScriptable = dialogueScriptable; // assign used scriptable object
        _npcName = npcName; // we need teh naem
        dialogueText.gameObject.SetActive(true);
        localTextProcessing();
        PlayerController.Instance.BlockInput();
        _inputActions.Enable();
    }


    // opens panel and sets text according to _npcText
    private void localTextProcessing()
    {
        _localDialogueProgress += 1;
        // shoutouts to scriptable objects not being able to have their members be accessible by fields
        switch (_localDialogueProgress)
        {
            case 1:
                _npcText = usedDialogueScriptable.text1;
                break;
            case 2:
                _npcText = usedDialogueScriptable.text2;
                break;
            case 3:
                _npcText = usedDialogueScriptable.text3;
                break;
            case 4:
                _npcText = usedDialogueScriptable.text4;
                break;
            case 5:
                _npcText = usedDialogueScriptable.text5;
                break;
            case 6:
                _npcText = usedDialogueScriptable.text6;
                break;
            case 7:
                _npcText = usedDialogueScriptable.text7;
                break;
            case 8:
                _npcText = usedDialogueScriptable.text8;
                break; // set text depending on dialogue progress, can't access the field via concatenation of "text" and localprogress because scrobjs don't use static fields
        }

        stopChat = false; // make sure chat's not stopped from previous convo
        
        if ((_npcText == "") || (_npcText == null)) // stop cond, when next dialogue is empty
        {
            stopChat = true;
            hidePanel();
            Reset();
        }

        if (!bg.gameObject.activeSelf) // if panel is off
        {
            // scale down beforehand so it can get scaled up once visible
            bg.DOFade(0f, 0f);
            dialogueText.DOFade(0f, 0f);

            // show panel
            bg.gameObject.SetActive(true);

            // scale up
            bg.DOFade(1f, 0.2f);
            dialogueText.DOFade(1f, 0.2f);
        }

        // set text
        dialogueText.SetText(_npcText);

        // play sound, use soundname by global and local progress and name
        // get sound from filesys and concat prog to chose
        if(!stopChat) // if chat doesn't need to stop
        {
            dialogueClip = Resources.Load<AudioClip>("Dialogue/" + _npcName + GameManager.GlobalProgress + _localDialogueProgress);
            GameObject.Find(_npcName).GetComponentInChildren<AudioSource>().clip = dialogueClip; // play said sound at npc pos
            GameObject.Find(_npcName).GetComponentInChildren<AudioSource>().Stop();
            GameObject.Find(_npcName).GetComponentInChildren<AudioSource>().Play();
        }
    }

    private void hidePanel(bool canIncreaseGlobalProgress = true) // hidePanel, what does it do? :flushed: CHANGE GLOBAL PROGRESS HERE IF WANTED HELP YO
    {
        bg.gameObject.SetActive(false);
        dialogueText.gameObject.SetActive(false);
        if(_npcName != null && GameObject.Find(_npcName) != null && GameObject.Find(_npcName).GetComponentInChildren<AudioSource>() != null)
            GameObject.Find(_npcName).GetComponentInChildren<AudioSource>().Stop();
        if (canIncreaseGlobalProgress && (_npcName == "chad") && (GameManager.GlobalProgress % 2 == 0))
        {
            GameManager.GlobalProgress += 1;
        }
    }

    private void Reset()
    {
        bg.DOFade(0f, 0.2f);
        dialogueText.DOFade(0f, 0.2f);
        Invoke("hidePanel", 0.2f);
        PlayerController.Instance.UnblockInput();
        _inputActions.Disable();
        _npcText = null;
        _npcName = null;
        _localDialogueProgress = 0;
        OnDialogueEnded?.Invoke();
    }
}
