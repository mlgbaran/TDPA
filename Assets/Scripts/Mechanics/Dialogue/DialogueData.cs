using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDialogueData", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Header("Speakers in this conversation")]
    public List<SpeakerInfo> participants;

    [Header("Is this a Phone Texting Dialogue?")]
    public bool isSMS = false;

    [Header("Lines in this conversation")]
    public List<DialogueLine> lines;


}
