using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [Header("Which speaker from 'participants' is talking?")]
    public int speakerIndex;  // references DialogueData.participants[speakerIndex]

    [Header("Optional: who are they talking to?")]
    public int targetIndex = -1; // references DialogueData.participants[targetIndex]

    [TextArea]
    public string text;

    [Header("Actions to perform after showing this line")]
    public DialogueAction[] actions;

    public bool unskippable = false;

}