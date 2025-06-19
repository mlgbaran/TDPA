using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    public Transform cameraTransform; // Reference to the camera
    public float parallaxFactor = 0.1f; // Adjust for how much slower the background moves
    private Vector3 lastCameraPosition; // Track the last camera position

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform; // Assign the main camera if not set
        }

        // Initialize the last camera position
        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        // Calculate how far the camera moved since the last frame
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        // Move the background by a fraction of the camera's movement
        transform.position += new Vector3(deltaMovement.x * parallaxFactor, deltaMovement.y * parallaxFactor, 0);

        // Update the last camera position
        lastCameraPosition = cameraTransform.position;
    }
}
