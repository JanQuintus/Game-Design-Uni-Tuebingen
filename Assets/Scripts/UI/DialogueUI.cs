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
    [SerializeField] private TMP_Text choice1;
    [SerializeField] private TMP_Text choice2;
    [SerializeField] private TMP_Text choice3;
    [SerializeField] private TMP_Text choice4;
    [SerializeField] private TMP_Text dialogText;

    private DialogueScriptableObject _currentDialogue;

    private PlayerInputActions _inputActions;

    // how many choices to display
    private int _displayChoiceAmt = 1;

    // text components
    private string _playerText1;
    private string _playerText2;
    private string _playerText3;
    private string _playerText4;
    private string _npcText;
    private string _textToReturn;
    private bool _hasGlobalTrimmed = false; // set to true once global culling has set place
    private bool _choosingDialogue = false; // set to true once we have text choices

    public bool IsOpen() => bg.gameObject.activeSelf;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        bg.gameObject.SetActive(false);
        choice1.gameObject.SetActive(false);
        choice2.gameObject.SetActive(false);
        choice3.gameObject.SetActive(false);
        choice4.gameObject.SetActive(false);
        dialogText.gameObject.SetActive(false);
        _inputActions = new PlayerInputActions(); // interaction decl
        _inputActions.UI.Interact.performed += (InputAction.CallbackContext context) => // define context as interact
        {
            if (context.performed) // on interact
            {
                if (_choosingDialogue)
                {
                    chooseDialogue();
                }
                else
                {
                    _textToReturn = _npcText;
                    loadStringByGlobalProgress(); // parse string based on global progress ( see below, culls on progress before delimiter )
                    localTextProcessing(); // parse string based on conversation path progress ( see below, culls per press )
                }
            }
        };
        _inputActions.UI.Cancel.performed += (InputAction.CallbackContext context) =>
        {
            if (context.performed && bg.gameObject.activeSelf)
                hidePanel();
        };
    }

    public void StartDialogue(DialogueScriptableObject dialogueScriptable)
    {
        if (_currentDialogue != null && _currentDialogue != dialogueScriptable)
            Reset();

        _currentDialogue = dialogueScriptable;
        _npcText = _currentDialogue.npcText;
        dialogText.gameObject.SetActive(true);
        if (_choosingDialogue)
        {
            chooseDialogue();
        }
        else
        {
            _textToReturn = _npcText;
            loadStringByGlobalProgress(); // parse string based on global progress ( see below, culls on progress before delimiter )
            localTextProcessing(); // parse string based on conversation path progress ( see below, culls per press )
        }
        PlayerController.Instance.BlockInput();
        _inputActions.Enable();
    }

    // SIDE NOTE - if we want to have more complex branches implement progress counter and use ascii offset for further letters ( we don't )
    // chooses the correct path for the npc text that we would like to traverse corresponding to the answer of the player
    private void chooseDialogue()
    {
        string currentSelect = EventSystem.current.currentSelectedGameObject.name;
        int currentSelectInt = currentSelect[6] - '0'; // le choice
        int selectDelimiter;
        switch (currentSelectInt)
        {
            case 1:
                selectDelimiter = _npcText.IndexOf("@a"); // get index according to choice    
                _npcText = _npcText.Remove(0, (selectDelimiter + 4));
                break;
            case 2:
                selectDelimiter = _npcText.IndexOf("@b");
                _npcText = _npcText.Remove(0, (selectDelimiter + 4));
                break;
            case 3:
                selectDelimiter = _npcText.IndexOf("@c");
                _npcText = _npcText.Remove(0, (selectDelimiter + 4));
                break;
            case 4:
                selectDelimiter = _npcText.IndexOf("@d");
                _npcText = _npcText.Remove(0, (selectDelimiter + 4));
                break;
        }
        _choosingDialogue = false; // set _choosingDialogue false
        hideChoices(); // call hideChoices() 
        _textToReturn = _npcText; // setting the _textToReturn
        localTextProcessing(); // call localTextProcessing
    }

    // parses string according to global progress where only the substring we need is left, EDIT THIS DEPENDING ON HOW WE USE GLOBALPROGRESS
    private void loadStringByGlobalProgress()
    {
        switch (GameManager.GlobalProgress)
        {
            case int n when (n < 100):
                break;

            case int n when (n >= 100):
                if (!_hasGlobalTrimmed)
                {
                    int i = _npcText.IndexOf("$a."); // get index to first global delimiter ( a for 100 and so on, change delim char depending on stuff )
                    _npcText = _npcText.Remove(0, (i + 3)); // parse by culling up to the delimiter
                    _hasGlobalTrimmed = true; // only trim once
                }
                break;
        }
    }

    private bool checkNextDelimiter() // true for no text branch, used in localTextProcessing(), compares index of next delimiters
    {
        int messageDelimiter = _npcText.IndexOf("|");
        int pathDelimiter = _npcText.IndexOf("@");
        if (pathDelimiter != -1)
        {
            return (messageDelimiter < pathDelimiter);
        }

        return true;
    }

    // parses string, looks for end and choices, calls editPanel() or hidePanel() or choiceProcessing() depending on what's coming up in the string
    private void localTextProcessing()
    {
        _displayChoiceAmt = 1; // set amount of choices to 1

        if (_npcText[0] == '<') // the time that only < is left ( end dialogue case )
        { // scale down and hide panel, reset stuff ( see below )
            bg.DOFade(0f, 0.2f);
            dialogText.DOFade(0f, 0.2f);
            choice1.DOFade(0f, 0.2f);
            choice2.DOFade(0f, 0.2f);
            choice3.DOFade(0f, 0.2f);
            choice4.DOFade(0f, 0.2f);
            Invoke("hidePanel", 0.2f);
        }

        else if (checkNextDelimiter()) // as long as no choices coming up
        {
            int nextDelim = _npcText.IndexOf("|");
            _textToReturn = _npcText.Remove(nextDelim); // cuts out everything from delimiter HERE
            _npcText = _npcText.Remove(0, (nextDelim + 1)); // cuts out everything up to including delimiter

            // call text return f
            editPanel();
        }

        else // otherwise there's a choice delimiter, so call choice f
        {
            choiceProcessing();
        }
    }

    // gets amt of choices for the choice panel, calls editPanelChoice()
    private void choiceProcessing()
    {
        int choiceDelim = _npcText.IndexOf("@");
        _textToReturn = _npcText.Remove(choiceDelim); // cuts out everything from delimiter
        _npcText = _npcText.Remove(0, choiceDelim); // cuts out everything up to delimiter
        _displayChoiceAmt = (int)char.GetNumericValue(_npcText[2]); // @x2. where 2 / 4 is the amount of choices, so we use the 2/4 here

        editPanelChoice(); // open the panels
    }

    // sets text according to _textToReturn, opens answer panels, autoselects choice1, enables dialogue flag
    private void editPanelChoice()
    {

        // set npc text
        dialogText.SetText(_textToReturn);

        // set ptexts
        _playerText1 = _currentDialogue.playerText.Remove(_currentDialogue.playerText.IndexOf("|")); // everything up to first delim NEW
        _playerText2 = _currentDialogue.playerText.Remove(0, (_currentDialogue.playerText.IndexOf("|") + 1)); // everything from first delim


        // only if there's 4 choices to say things, enable the other 2
        if (_displayChoiceAmt == 4)
        {
            // set ptext
            _playerText4 = _currentDialogue.playerText.Remove(0, (_currentDialogue.playerText.IndexOf("/") + 1)); // everything from last delim
            _playerText2 = _playerText2.Remove(_playerText2.IndexOf("/")); // trim the end from here too, so we can use it to set _playerText3 more easily
            _playerText3 = _playerText2.Remove(0, (_playerText2.IndexOf("|") + 1)); // everything from remaining "|" delim in _playerText2
            _playerText2 = _playerText2.Remove(_playerText2.IndexOf("|")); // only contain up to next delim

            // enable panels
            choice3.gameObject.SetActive(true);
            choice3.DOFade(1f, 0.2f);
            choice3.text = _playerText3;
            choice4.gameObject.SetActive(true);
            choice4.DOFade(1f, 0.2f);
            choice4.text = _playerText4;
        }

        // enable panels
        choice1.gameObject.SetActive(true);
        choice1.DOFade(1f, 0.2f);
        choice1.text = _playerText1;
        choice2.gameObject.SetActive(true);
        choice2.DOFade(1f, 0.2f);
        choice2.text = _playerText2;

        // select choice 1
        EventSystem.current.SetSelectedGameObject(choice1.gameObject);

        // set dialogue flag
        _choosingDialogue = true;
    }

    // opens panel and sets text according to _textToReturn
    private void editPanel()
    {
        if (!bg.gameObject.activeSelf) // if panel is off
        {
            // scale down beforehand so it can get scaled up once visible
            bg.DOFade(0f, 0f);
            choice1.DOFade(0f, 0f);
            choice2.DOFade(0f, 0f);
            choice3.DOFade(0f, 0f);
            choice4.DOFade(0f, 0f);
            dialogText.DOFade(0f, 0f);

            // show panel
            bg.gameObject.SetActive(true);

            // scale up
            bg.DOFade(1f, 0.2f);
            dialogText.DOFade(1f, 0.2f);
        }

        // set text
        dialogText.SetText(_textToReturn);
    }

    private void hidePanel() // hidePanel, what does it do? :flushed: CHANGE GLOBAL PROGRESS HERE IF WANTED ( remove feature? )
    {
        bg.gameObject.SetActive(false);
        hideChoices();
        dialogText.gameObject.SetActive(false);
        Reset();
    }

    private void Reset()
    {
        // progress ( we can change this individually without affecting global p so we can have people say multiple dialogue lines on same progress without effects on global progress
        _hasGlobalTrimmed = false;

        // also reset the string in case we wanna talk again
        _npcText = _currentDialogue.npcText;
        PlayerController.Instance.UnblockInput();
        _inputActions.Disable();

        _currentDialogue = null;

        OnDialogueEnded?.Invoke();
    }

    private void hideChoices() // self-xpl
    {
        choice1.gameObject.SetActive(false);
        choice2.gameObject.SetActive(false);
        choice3.gameObject.SetActive(false);
        choice4.gameObject.SetActive(false);
    }
}
