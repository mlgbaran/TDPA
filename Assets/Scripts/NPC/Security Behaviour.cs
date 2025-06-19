using UnityEngine;

public class SecurityBehaviour : MonoBehaviour
{
    public Animator animator;         // Security's Animator
    public float detectionRadius = 0.22f;
    public LayerMask playerLayer;

    [Header("Dialogue")]
    public DialogueManager dialogueManager;  // Drag your DialogueManager from the scene here
    public DialogueData securityDialogue;    // Drag a DialogueData asset (with lines for Security) here
    public DialogueData securityDialogue2;    // Drag a DialogueData asset (with lines for Security) here
    public DialogueData securityDialogue3;

    private bool playerInRange = false;
    private bool secondTry = false;
    private bool accepted = false;
    private bool cameback = false;

    void Update()
    {
        // Is the player within the detectionRadius?
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        if (player && !playerInRange)
        {
            playerInRange = true;
            if (!secondTry)
            {
                dialogueManager.StartDialogue(securityDialogue);
            }
            else if (!accepted)
            {
                dialogueManager.StartDialogue(securityDialogue2);
            }
            else if (cameback)
            {

                TriggerLetInAnimation();
            }




        }
        else if (!player && playerInRange)
        {
            playerInRange = false;
            Debug.Log("exiting");

            if (accepted && !cameback)
            {

                dialogueManager.StartDialogue(securityDialogue3);
                TriggerBackToIdleAnimation();

                cameback = true;

            }
            else if (accepted && cameback)
            {

                TriggerBackToIdleAnimation();

            }




        }



    }

    public void LetSkipDialogueLine()
    {
        if (dialogueManager != null)
        {
            dialogueManager.OnAnimationFinished();
        }
    }


    // Called by DialogueManager when user picks "Baran"
    public void TriggerAcceptAnimation()
    {
        animator.SetTrigger("SecurityAccept");
        Debug.Log("SecurityAccept triggered!");
    }

    // Called by DialogueManager when user picks "Kivanc"
    public void TriggerRejectAnimation()
    {
        animator.SetTrigger("SecurityReject");
        Debug.Log("SecurityReject triggered!");
    }

    public void TriggerBackToIdleAnimation()
    {
        animator.SetTrigger("BackToIdle");
        Debug.Log("BackToIdle triggered!");
    }

    public void TriggerLetInAnimation()
    {
        animator.SetTrigger("LetIn");
        Debug.Log("LetIn triggered!");
    }

    public void OnLetInAnimationFinished()
    {

        animator.SetTrigger("LetInFinished");
        Debug.Log("LetInFinished triggered!");
    }

    // Called at the end of the "SecurityAccept" animation via Animation Event
    public void OnAcceptAnimationFinished()
    {
        animator.SetTrigger("AcceptFinished");
        Debug.Log("AcceptFinished triggered!");
        LetSkipDialogueLine();
        dialogueManager.GoToNextLine();
        accepted = true;
    }

    // Called at the end of the "SecurityReject" animation via Animation Event
    public void OnRejectAnimationFinished()
    {
        animator.SetTrigger("RejectFinished");
        Debug.Log("RejectFinished triggered!");
        LetSkipDialogueLine();
        dialogueManager.GoToNextLine();
        secondTry = true;
    }

    public void OnBackToIdleFinished()
    {
        animator.SetTrigger("BackToIdleFinished");
        Debug.Log("BackToIdleFinished triggered!");
        //cameback = true;
    }

    // Visualize the detection circle in the Scene
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
