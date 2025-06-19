using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Suppose this script is attached to the Player, which also has the Animator.
// And you have a public DialogueManager reference, assigned in the Inspector.
public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField]
    private DialogueManager dialogueManager;
    public PlayerMovement playerMovement;
    public Animator playerAnimator;

    public void OnPhonePutDown()
    {
        Debug.Log("Animation event fired: phone put down.");

        playerMovement.enabled = true;
        OnCutsceneFinished();

    }

    public void OnCutsceneFinished()
    {
        playerAnimator.SetBool("isInCutscene", false);
    }

    public void OnCutsceneStarted()
    {
        playerAnimator.SetBool("isInCutscene", true);
    }
}

