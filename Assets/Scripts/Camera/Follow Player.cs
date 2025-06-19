using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;       // Main camera
    [SerializeField] private Transform player; // The player transform

    [Header("Dead Zone (Screen Space)")]
    [Tooltip("Size of the rectangular dead zone at the center of the screen, in pixels.")]
    public Vector2 deadZoneSize = new Vector2(400f, 200f);

    [Header("Camera Offset in World Space")]
    [Tooltip("Maximum distance in world units the camera can offset from the player.")]
    public float maxOffset = 3f;

    [Header("Smoothing")]
    [Tooltip("SmoothDamp duration. Lower = more snappy, higher = floaty.")]
    public float smoothTime = 0.2f;
    private Vector3 currentVelocity; // internal for SmoothDamp

    [Header("Dialogue Mode")]
    public bool isDialogueMode = false;
    [Tooltip("If true, we ignore mouse offset entirely during dialogue.")]
    public bool ignoreMouseInDialogue = true;

    [Header("Side View")]
    public bool sideView = false;
    public float sideViewOffsetX = 0;
    public float sideViewOffsetY = 0;

    // For drawing gizmos, we store the corners each frame
    private Vector3[] deadZoneWorldCorners = new Vector3[4];

    void Update()
    {
        if (!player || !cam)
            return;

        if (sideView)
        {
            Vector3 sideViewPos = new Vector3(
                player.position.x + sideViewOffsetX,
                player.position.y + sideViewOffsetY,
                transform.position.z  // keep current camera Z
            );

            // Just set the position directly, no SmoothDamp
            transform.position = sideViewPos;
            return;
        }

        // If we are in dialogue mode and ignoring the mouse, just follow the player directly
        if (isDialogueMode && ignoreMouseInDialogue)
        {
            SmoothFollowPlayerDirectly();
            return;
        }

        // 1) Build a Rect in screen space for the dead zone
        //    Centered at (Screen.width/2, Screen.height/2)
        //    Size = deadZoneSize
        Rect deadZoneRect = new Rect(
            (Screen.width - deadZoneSize.x) * 0.5f,
            (Screen.height - deadZoneSize.y) * 0.5f,
            deadZoneSize.x,
            deadZoneSize.y
        );

        // 2) Check if mouse is within that rectangle
        Vector2 mousePos = Input.mousePosition;
        bool isInsideDeadZone = deadZoneRect.Contains(mousePos);

        if (isInsideDeadZone)
        {
            // If the mouse is inside the dead zone, don't offset the camera
            SmoothFollowPlayerDirectly();
        }
        else
        {
            // The mouse is outside the dead zone, so let's offset the camera
            // a) We calculate how far the mouse is beyond the rect boundary in X and Y
            float offsetX = 0f;
            float offsetY = 0f;

            // Left edge
            if (mousePos.x < deadZoneRect.xMin)
            {
                // how far from the left edge
                float distLeft = deadZoneRect.xMin - mousePos.x;
                // how far to the left screen edge from rect edge
                float maxDistLeft = deadZoneRect.xMin - 0f;
                float ratio = Mathf.Clamp01(distLeft / maxDistLeft);
                offsetX = -ratio; // negative for left
            }
            // Right edge
            else if (mousePos.x > deadZoneRect.xMax)
            {
                float distRight = mousePos.x - deadZoneRect.xMax;
                float maxDistRight = Screen.width - deadZoneRect.xMax;
                float ratio = Mathf.Clamp01(distRight / maxDistRight);
                offsetX = ratio; // positive for right
            }

            // Bottom edge
            if (mousePos.y < deadZoneRect.yMin)
            {
                float distBottom = deadZoneRect.yMin - mousePos.y;
                float maxDistBottom = deadZoneRect.yMin - 0f;
                float ratio = Mathf.Clamp01(distBottom / maxDistBottom);
                offsetY = -ratio; // negative for bottom
            }
            // Top edge
            else if (mousePos.y > deadZoneRect.yMax)
            {
                float distTop = mousePos.y - deadZoneRect.yMax;
                float maxDistTop = Screen.height - deadZoneRect.yMax;
                float ratio = Mathf.Clamp01(distTop / maxDistTop);
                offsetY = ratio; // positive for top
            }

            // b) Combine offsetX/offsetY into a 2D direction in screen space
            //    Then convert that to world space
            Vector2 screenDir = new Vector2(offsetX, offsetY);
            float screenDirMagnitude = screenDir.magnitude;
            if (screenDirMagnitude > 1f)
            {
                screenDir.Normalize();
            }

            // c) The offset in world space = direction * maxOffset
            //    We'll transform that screen direction into a rough world direction
            //    by mapping screen center -> screen center + screenDir
            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f,
                Mathf.Abs(cam.transform.position.z - player.position.z));
            Vector3 screenCenterPlusDir = screenCenter + new Vector3(screenDir.x, screenDir.y, 0f) * 100f;
            // '100f' is arbitrary scale. We just need a direction vector in world space.

            // Convert both points to world space
            Vector3 worldCenter = cam.ScreenToWorldPoint(screenCenter);
            Vector3 worldCenterPlusD = cam.ScreenToWorldPoint(screenCenterPlusDir);

            // The direction in world space
            Vector3 worldDir = (worldCenterPlusD - worldCenter);
            worldDir.z = 0f; // flatten if 2D

            // Final offset in world space
            Vector3 desiredOffset = worldDir.normalized * (maxOffset * screenDirMagnitude);

            // d) Our desired camera position = player's position + offset
            Vector3 desiredPos = player.position + desiredOffset;

            desiredPos.z = transform.position.z; // keep current Z if top-down or 2D

            // e) SmoothDamp to the desired position
            transform.position = Vector3.SmoothDamp(
                transform.position,
                desiredPos,
                ref currentVelocity,
                smoothTime
            );
        }

        // 3) For gizmos, we compute the corners of the dead zone in world space
        UpdateDeadZoneWorldCorners(deadZoneRect);
    }

    /// <summary>
    /// Smoothly follow the player's position (no mouse offset).
    /// </summary>
    private void SmoothFollowPlayerDirectly()
    {
        Vector3 targetPos = player.position;

        // Apply side-view offset if enabled
        if (sideView)
        {
            targetPos.x += sideViewOffsetX;
            targetPos.y += sideViewOffsetY;
        }

        // Keep camera's current Z
        targetPos.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref currentVelocity,
            smoothTime
        );
    }

    /// <summary>
    /// Convert the 2D screen-space deadZoneRect into 4 corners in world space at the player's Z.
    /// We'll store them in deadZoneWorldCorners[0..3].
    /// </summary>
    private void UpdateDeadZoneWorldCorners(Rect deadZoneRect)
    {
        if (!player || !cam) return;

        float zDistance = Mathf.Abs(cam.transform.position.z - player.position.z);

        // "Top-left" in screen coords
        Vector3 topLeft = new Vector3(deadZoneRect.xMin, deadZoneRect.yMax, zDistance);
        Vector3 topRight = new Vector3(deadZoneRect.xMax, deadZoneRect.yMax, zDistance);
        Vector3 bottomRight = new Vector3(deadZoneRect.xMax, deadZoneRect.yMin, zDistance);
        Vector3 bottomLeft = new Vector3(deadZoneRect.xMin, deadZoneRect.yMin, zDistance);

        deadZoneWorldCorners[0] = cam.ScreenToWorldPoint(topLeft);
        deadZoneWorldCorners[1] = cam.ScreenToWorldPoint(topRight);
        deadZoneWorldCorners[2] = cam.ScreenToWorldPoint(bottomRight);
        deadZoneWorldCorners[3] = cam.ScreenToWorldPoint(bottomLeft);
    }

    /// <summary>
    /// Draws lines in the Scene view for debugging.
    /// This shows an approximate rectangle in world space that corresponds
    /// to the center screen dead zone (at the player's Z).
    /// </summary>
    private void OnDrawGizmos()
    {
        // If we haven't updated corners yet this frame, or if references are missing, bail.
        if (!cam || !player || deadZoneWorldCorners == null || deadZoneWorldCorners.Length < 4)
            return;

        Gizmos.color = Color.green;
        // corners: 0=topLeft, 1=topRight, 2=bottomRight, 3=bottomLeft
        Gizmos.DrawLine(deadZoneWorldCorners[0], deadZoneWorldCorners[1]);
        Gizmos.DrawLine(deadZoneWorldCorners[1], deadZoneWorldCorners[2]);
        Gizmos.DrawLine(deadZoneWorldCorners[2], deadZoneWorldCorners[3]);
        Gizmos.DrawLine(deadZoneWorldCorners[3], deadZoneWorldCorners[0]);
    }
}
