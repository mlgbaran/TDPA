using UnityEngine;

public class CrowMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float flySpeed = 2f; // Speed at which the crow flies

    [Header("Fly Target")]
    public Transform flyTarget; // Assign the "Crow Fly Direction" GameObject here

    [Header("Scaling Settings")]
    public float scaleDownDuration = 1f; // Time in seconds to scale down
    public Vector3 targetScale = Vector3.zero; // Final scale after scaling down

    private Animator animator;
    private bool isFlying = false;

    private Vector3 initialScale;
    private float initialDistance;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("CrowMovement: Animator component not found on " + gameObject.name);
        }

        if (flyTarget == null)
        {
            Debug.LogWarning("CrowMovement: Fly Target not assigned for " + gameObject.name);
        }
    }

    void Update()
    {
        if (isFlying && flyTarget != null)
        {
            MoveCrowTowardsTarget();
        }
    }

    // Public method to start flying
    public void StartFlying()
    {
        isFlying = true;
        if (animator != null)
        {
            animator.SetTrigger("StartFly"); // Ensure "StartFly" trigger exists in Animator
        }
    }

    // Public method to stop flying (optional)
    public void StopFlying()
    {
        isFlying = false;
        if (animator != null)
        {
            animator.ResetTrigger("StartFly");
            // Optionally, transition back to idle animation
        }
    }

    private void MoveCrowTowardsTarget()
    {
        // Calculate direction towards the target
        Vector3 direction = (flyTarget.position - transform.position).normalized;

        // Move the crow towards the target
        transform.Translate(direction * flySpeed * Time.deltaTime, Space.World);

        // Calculate remaining distance
        float distance = Vector3.Distance(transform.position, flyTarget.position);

        // Scale down proportionally to the remaining distance
        if (initialDistance > 0)
        {
            float scaleFactor = distance / initialDistance;
            scaleFactor = Mathf.Clamp(scaleFactor, 0f, 1f); // Ensure scaleFactor stays between 0 and 1
            transform.localScale = initialScale * scaleFactor;
        }

        // Check if the crow has reached the target
        if (distance < 0.1f)
        {
            Destroy(gameObject); // Destroy the crow upon reaching the target
        }
    }
}
