using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class DummyDialogue : AInteractive
{
    /* what to watch for when using applying this script to other NPCs:
     * name of this script
     * l.32, assign NPC ScriptableObject
     * l.96, choose correct globalProgress for when we would like the dialogue to change in cases, if we have a range we can also use if-else
     * l.99, choose correct delimiter for globalProgresses
     */

    // side note - if this somehow does weird things we can remove static properties off the scriptable object, and load the values in start() or awake() instead of setting them right here

    // Panel for active dialogue box initialize
    public RectTransform DialoguePanel;
    public RectTransform ChoicePanel1;
    public RectTransform ChoicePanel2;
    public RectTransform ChoicePanel3;
    public RectTransform ChoicePanel4;

    // how many choices to display
    private int _displayChoiceAmt = 1;

    // text components
    private string _playerText1;
    private string _playerText2;
    private string _playerText3;
    private string _playerText4;
    private string _npcText = DummyDialogueScriptableObject.dummyText; // assign per NPC
    private string _textToReturn;
    private int _globalProgress = GameManager.globalProgress;
    private bool _hasGlobalTrimmed = false; // set to true once global culling has set place
    private bool _choosingDialogue = false; // set to true once we have text choices


    public override void Interact(bool isRelease) // on Interact
    {
        if (!isRelease)
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

    // parses string according to global progress where only the substring we need is left
    private void loadStringByGlobalProgress()
    {
        switch (_globalProgress)
        {
            case 0:
                break;

            case 1:
                if (!_hasGlobalTrimmed)
                {
                    int i = _npcText.IndexOf("$a."); // get index to first global delimiter
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
            DialoguePanel.DOScaleX(0.1f, 0.2f);
            DialoguePanel.DOScaleY(0.1f, 0.2f);
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
    private void editPanelChoice() // WE ARE HERE
    {
       
        // set npc text
        DialoguePanel.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = _textToReturn;

        // set ptexts
        
        _playerText1 = DummyDialogueScriptableObject.playerDummyText.Remove(DummyDialogueScriptableObject.playerDummyText.IndexOf("|")); // everything up to first delim
        _playerText2 = DummyDialogueScriptableObject.playerDummyText.Remove(0, (DummyDialogueScriptableObject.playerDummyText.IndexOf("|") + 1)); // everything from first delim
        

        // only if there's 4 choices to say things, enable the other 2
        if (_displayChoiceAmt == 4)
        {
            // set ptext
            _playerText4 = DummyDialogueScriptableObject.playerDummyText.Remove(0, (DummyDialogueScriptableObject.playerDummyText.IndexOf("/") + 1)); // everything from last delim
            _playerText2 = _playerText2.Remove(_playerText2.IndexOf("/")); // trim the end from here too, so we can use it to set _playerText3 more easily
            _playerText3 = _playerText2.Remove(0, (_playerText2.IndexOf("|") + 1)); // everything from remaining "|" delim in _playerText2
            _playerText2 = _playerText2.Remove(_playerText2.IndexOf("|")); // only contain up to next delim

            // enable panels
            ChoicePanel3.gameObject.SetActive(true);
            ChoicePanel3.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = _playerText3;
            ChoicePanel4.gameObject.SetActive(true);
            ChoicePanel4.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = _playerText4;
        }

        // enable panels
        ChoicePanel1.gameObject.SetActive(true);
        ChoicePanel1.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = _playerText1;
        ChoicePanel2.gameObject.SetActive(true);
        ChoicePanel2.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = _playerText2;

        // select choice 1
        EventSystem.current.SetSelectedGameObject(ChoicePanel1.gameObject);

        // set dialogue flag
        _choosingDialogue = true;
    }

    // opens panel and sets text according to _textToReturn
    private void editPanel()
    {
        if (!DialoguePanel.gameObject.activeSelf) // if panel is off
        {
            // scale down beforehand so it can get scaled up once visible
            DialoguePanel.DOScaleX(0.1f, 0f);
            DialoguePanel.DOScaleY(0.1f, 0f);

            // show panel
            DialoguePanel.gameObject.SetActive(true);

            // scale up
            DialoguePanel.DOScaleX(1f, 0.2f);
            DialoguePanel.DOScaleY(1f, 0.2f);
        }
        
        // set text
        DialoguePanel.gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = _textToReturn;
    }

    private void hidePanel() // hidePanel, what does it do? :flushed: CHANGE GLOBAL PROGRESS HERE IF WANTED
    {
        DialoguePanel.gameObject.SetActive(false);

        // progress ( we can change this individually without affecting global p so we can have people say multiple dialogue lines on same progress without effects on global progress
        _globalProgress = 1;
        _hasGlobalTrimmed = false;

        // also reset the string in case we wanna talk again
        _npcText = DummyDialogueScriptableObject.dummyText;
    }

    private void hideChoices() // self-xpl
    {
        ChoicePanel1.gameObject.SetActive(false);
        ChoicePanel2.gameObject.SetActive(false);
        if (_displayChoiceAmt == 4)
        {
            ChoicePanel3.gameObject.SetActive(false);
            ChoicePanel4.gameObject.SetActive(false);
        }
    }
}


