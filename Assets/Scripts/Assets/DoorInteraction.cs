using UnityEngine;
using System.Collections;
using UnityEditor;

public class DoorInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator doorAnimator;      // Door's animator
    [SerializeField] private Animator playerAnimator;    // Player's animator
    [SerializeField] private Transform playerTransform;  // Player's transform
    [SerializeField] private Collider2D doorCollider;    // The blocking collider

    [Header("Settings")]
    [SerializeField] private float detectionRadius = 2f;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    [SerializeField] private float moveDuration = 1f;

    [SerializeField] private Vector3 startPosRight;
    [SerializeField] private Vector3 endPosRight;

    [SerializeField] private Vector3 startPosLeft;
    [SerializeField] private Vector3 endPosLeft;

    // Booleans for logic

    // If you want to block repeated triggers until the animation ends, you can track that too.
    // private bool isAnimating = false; 

    // We'll assume the player's script or animator sets these:
    // - DirectionX => +1 (facing right), -1 (facing left)
    // - Horizontal => +1 (moving right), 0 (idle), -1 (moving left)
    private float directionX => playerAnimator.GetFloat("DirectionX");
    private float horizontal => playerAnimator.GetFloat("Horizontal");

    private void Update()
    {
        // Check if the player is close enough
        float dist = Vector2.Distance(transform.position, playerTransform.position);
        if (dist <= detectionRadius && Input.GetKeyDown(interactionKey))
        {
            TryOpenDoor();
        }
    }

    private void TryOpenDoor()
    {
        // If it's already open, you might want to close it or do nothing.
        //if (isOpen)
        //    return;  // or handle closing logic

        // 1. Figure out if the door is to the player's left or right
        float xDiff = transform.position.x - playerTransform.position.x;
        bool doorOnRight = xDiff > 0f;

        // 2. Check if player is facing or moving in that direction
        // For door on right, we want directionX > 0 OR horizontal > 0
        // For door on left,  directionX < 0 OR horizontal < 0
        if (doorOnRight)
        {
            bool isFacingRightOrRunningRight = directionX > 0 || horizontal > 0;
            if (!isFacingRightOrRunningRight)
            {
                // Player isn't looking or moving right, so do not open
                return;
            }

            // Player is on left side of door, looking/moving right => open door to the right
            doorAnimator.SetTrigger("OpenDoorRight");
            playerAnimator.SetTrigger("OpenDoorRight");

            StartCoroutine(OpenDoorSequence(startPosRight, endPosRight));
        }
        else
        {
            // Door is on the left
            bool isFacingLeftOrRunningLeft = directionX < 0 || horizontal < 0;
            if (!isFacingLeftOrRunningLeft)
            {
                // Player isn't looking or moving left, so do not open
                return;
            }

            // Player is on right side of door, looking/moving left => open door to the left
            doorAnimator.SetTrigger("OpenDoorLeft");
            playerAnimator.SetTrigger("OpenDoorLeft");

            StartCoroutine(OpenDoorSequence(startPosLeft, endPosLeft));
        }

        playerAnimator.SetBool("isInCutscene", true);

    }

    private IEnumerator OpenDoorSequence(Vector3 startPos, Vector3 endPos)
    {
        // 1) Optionally position the player exactly at the startPos
        playerTransform.position = startPos;

        yield return StartCoroutine(MovePlayerFromTo(
            playerTransform.position,
            startPos,
            0.2f // short duration to avoid camera snapping
        ));

        // 2) Move from start to end over moveDuration
        yield return StartCoroutine(MovePlayerFromTo(startPos, endPos, moveDuration));

        // 3) Disable the collider so the player can pass through
        doorCollider.enabled = false;
    }

    private IEnumerator MovePlayerFromTo(Vector3 start, Vector3 end, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            playerTransform.position = Vector3.Lerp(start, end, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // snap to final position at the end
        playerTransform.position = end;
        playerAnimator.SetBool("isInCutscene", false);
    }




    // (Optional) if you want the door to close automatically after some time:
    // 1) Re-enable collider
    // 2) Trigger a "CloseDoor" animation, etc.

    // This shows the detection radius in the scene view for debugging


    // ...

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        // Draw lines (optional) between each startPos -> endPos
        Gizmos.DrawLine(startPosRight, endPosRight);
        Gizmos.DrawLine(startPosLeft, endPosLeft);

        // Already drawing spheres at those points? If not, add them:
        Gizmos.DrawSphere(startPosRight, 0.01f);
        Gizmos.DrawSphere(endPosRight, 0.01f);
        Gizmos.DrawSphere(startPosLeft, 0.01f);
        Gizmos.DrawSphere(endPosLeft, 0.01f);

#if UNITY_EDITOR
        Handles.Label(startPosRight + Vector3.up * 0.05f, "startPosRight");
        Handles.Label(endPosRight + Vector3.up * 0.05f, "endPosRight");
        Handles.Label(startPosLeft + Vector3.up * 0.05f, "startPosLeft");
        Handles.Label(endPosLeft + Vector3.up * 0.05f, "endPosLeft");
#endif

        // Now, actual draggable handles:
        if (!Application.isPlaying)
        {
            startPosRight = Handles.PositionHandle(startPosRight, Quaternion.identity);
            endPosRight = Handles.PositionHandle(endPosRight, Quaternion.identity);

            startPosLeft = Handles.PositionHandle(startPosLeft, Quaternion.identity);
            endPosLeft = Handles.PositionHandle(endPosLeft, Quaternion.identity);
        }
    }
#endif
}
