
using UnityEngine;

[System.Serializable]

public class FightStep
{
    public string animatorStateName;  // Name of the state/clip in Animator
    public bool isQTE;                // Does this step require a QTE?
    public KeyCode requiredKey;       // The key or button to press
    public float timeWindow;          // How long the player has to press it
    public float clipLength;          // If no QTE, how long the clip plays

    public string successTrigger;     // Animator trigger to set on success
    public string failTrigger;        // Animator trigger to set on failure
}