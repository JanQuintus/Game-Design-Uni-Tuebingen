using UnityEngine;

// asset creator
[CreateAssetMenu(fileName = "New Dialogue", menuName = "DialogueScriptableObject")]
public class DialogueScriptableObject : ScriptableObject
{
    // npc text contents ( split with | per answer, split with @x2. (x is a/b/c/d, 2/4 is amt of choices ) per conversation local path, split with $x. per conversation global path, paths end on |< )
    [TextArea(2, 8)] // global path delim chars must be chosen correctly
    public static string dummyText = "messsage1|message2" +
        "@a4.choice11|choice12|<" +
        "@b4.choice21|choice22|<" +
        "@c4.choice3|<" +
        "@d4.choice4|<" +
        "$a.global1|global2|<";

    // player choices ( split a la "message1|message2|message3/messgae4" or "message1|message2"; | is either only delim or first two delim followed by / )
    [TextArea(2, 8)]
    public static string playerDummyText = "message1|message2|message3/message4";

    // can cull these if necessary^ ( keep info on conventions though )
    [TextArea(2, 8)]
    [Tooltip("see DialogueScriptableObject for conventions")]
    public string npcText;
    [TextArea(2, 8)]
    public string playerText;
}