using UnityEngine;

public enum DialogueActionType
{
    TriggerAnimation,
    TriggerFunction
    // You could add more in the future: PlaySound, ChangeVariable, etc.
}

[System.Serializable]
public class DialogueAction
{
    public DialogueActionType actionType;

    [Header("Which participant does this action? (index in DialogueData.participants)")]
    public int targetIndex = -1; // e.g. if 0 => DialogueData.participants[0]

    [Header("For TriggerAnimation")]
    public string animationTriggerName; // The trigger to set on the target's Animator
}
