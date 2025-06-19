using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMS1Script : MonoBehaviour
{

    //public Animator animator;         // Security's Animator
    public float detectionRadius = 0.22f;
    public LayerMask playerLayer;
    public float detectionOffsetx = 0.1f;
    public float detectionOffsety = 0.1f;

    [Header("Dialogue")]
    public DialogueManager dialogueManager;  // Drag your DialogueManager from the scene here
    public DialogueData SMSDialogue;    // Drag a DialogueData asset (with lines for Security) here

    private Transform lookDirection;
    private Vector3 initialLookDirectionPosition;

    private bool playerInRange = false;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        if (player && !playerInRange)
        {
            playerInRange = true;
            dialogueManager.StartDialogue(SMSDialogue);
        }
    }

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
