using UnityEngine;

// asset creator
[CreateAssetMenu(fileName = "New Dialogue", menuName = "DialogueScriptableObject")]
public class DialogueScriptableObject : ScriptableObject
{
    // add strings, one obj per dialogue, name obj after globalprogress each line is dependant on localprogress, can just use names hehe
    [TextArea(2, 8)]
    public string text1;
    [TextArea(2, 8)]
    public string text2;
    [TextArea(2, 8)]
    public string text3;
    [TextArea(2, 8)]
    public string text4;
    [TextArea(2, 8)]
    public string text5;
    [TextArea(2, 8)]
    public string text6;
    [TextArea(2, 8)]
    public string text7;
    [TextArea(2, 8)]
    public string text8;
}