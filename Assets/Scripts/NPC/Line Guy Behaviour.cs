using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGuyBehaviour : MonoBehaviour
{
    //public Animator animator;         // Security's Animator
    public float detectionRadius = 0.22f;
    public LayerMask playerLayer;
    public float detectionOffsetx = 0.1f;
    public float detectionOffsety = 0.1f;

    [Header("Dialogue")]
    public DialogueManager dialogueManager;  // Drag your DialogueManager from the scene here
    public DialogueData lineGuyDialogue;    // Drag a DialogueData asset (with lines for Security) here

    private Transform lookDirection;
    private Vector3 initialLookDirectionPosition;

    private bool playerInRange = false;


    void Start()
    {
        // Find the LookDirection child
        lookDirection = transform.Find("LookDirection");
        if (lookDirection != null)
        {
            initialLookDirectionPosition = lookDirection.position;
        }
        else
        {
            Debug.LogError("LookDirection child not found!");
        }
    }

    void Update()
    {
        // Is the player within the detectionRadius?

        Vector2 detectionCenter = new Vector2(
                    transform.position.x + detectionOffsetx,
                    transform.position.y + detectionOffsety
                );

        // Check if the player is within the detectionRadius from the detectionCenter
        Collider2D player = Physics2D.OverlapCircle(detectionCenter, detectionRadius, playerLayer);
        if (player && !playerInRange)
        {
            playerInRange = true;
            dialogueManager.StartDialogue(lineGuyDialogue);
            if (dialogueManager != null)
            {
                dialogueManager.OnDialogueEnded += ResetLookDirectionPosition;
            }
        }
        else if (!player && playerInRange)
        {
            playerInRange = false;
            if (dialogueManager != null)
            {
                dialogueManager.OnDialogueEnded -= ResetLookDirectionPosition;
            }
        }

    }

    public void ResetLookDirectionPosition()
    {
        if (lookDirection != null)
        {
            lookDirection.position = initialLookDirectionPosition;
        }
    }

    // Visualize the detection circle in the Scene
    private void OnDrawGizmosSelected()
    {
        Vector3 detectionCenter = new Vector3(
            transform.position.x + detectionOffsetx,
            transform.position.y + detectionOffsety,
            transform.position.z // Keep the original z position
        );

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(detectionCenter, detectionRadius);
    }

}
