using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform; // Reference to the CameraPos or Virtual Camera
    [SerializeField] private Vector2 parallaxMultiplier = new Vector2(0.1f, 0.1f); // Parallax effect strength
    private Vector3 lastCameraPosition;

    private void Start()
    {
        if (!cameraTransform)
        {
            cameraTransform = Camera.main.transform; // Default to the main camera
        }
        lastCameraPosition = cameraTransform.position;
    }

    private void LateUpdate()
    {
        // Calculate how much the camera has moved since the last frame
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        // Apply parallax effect (only X and Y axes for 2D games)
        transform.position += new Vector3(deltaMovement.x * parallaxMultiplier.x, deltaMovement.y * parallaxMultiplier.y, 0);

        // Update the last camera position
        lastCameraPosition = cameraTransform.position;
    }
}