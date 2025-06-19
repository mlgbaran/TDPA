using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PoliceCar2DDoppler : MonoBehaviour
{
    [Header("Movement")]
    public Transform startPoint;
    public Transform endPoint;
    public float moveSpeed = 5f;

    [Header("Audio")]
    public AudioSource sirenAudio;
    public Transform player;  // Assign your player's Transform
    public float minDistance = 0f;    // e.g., 0 (or a small number) for closest point
    public float maxDistance = 20f;   // e.g., distance between startPoint & endPoint
    public float minPitch = 0.8f;
    public float maxPitch = 1.4f;
    public float minVolume = 0f;
    public float maxVolume = 1f;

    private bool movingToEnd = true;
    private float lastDistance = Mathf.Infinity;

    void Start()
    {
        // Just place the police car at the start
        if (startPoint != null) transform.position = startPoint.position;

        // Make sure the siren is looping and playing
        if (sirenAudio != null)
        {
            sirenAudio.loop = true;
            sirenAudio.Play();
        }

        // If you want the built-in 3D Doppler to do some pitch shifting automatically,
        // you can set sirenAudio.spatialBlend = 1f and sirenAudio.dopplerLevel = 1f.
        // But for a pure 2D approach, we’ll do everything manually below.
    }

    void Update()
    {
        // 1) Move the car between startPoint and endPoint
        MoveCar();

        // 2) Update the doppler-like audio effect
        UpdateDopplerAudio();
    }

    private void MoveCar()
    {
        if (startPoint == null || endPoint == null) return;

        float step = moveSpeed * Time.deltaTime;
        Vector3 targetPos = movingToEnd ? endPoint.position : startPoint.position;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        // Once the car reaches one end, switch direction
        if (Vector3.Distance(transform.position, targetPos) < 0.001f)
        {
            movingToEnd = !movingToEnd;
        }
    }

    private void UpdateDopplerAudio()
    {
        if (sirenAudio == null || player == null) return;

        // Current distance from car to player
        float distance = Vector2.Distance(transform.position, player.position);

        // Clamp distance to our min/max
        float clampedDist = Mathf.Clamp(distance, minDistance, maxDistance);

        // Volume goes from max at minDistance to min at maxDistance
        float distRatio = (clampedDist - minDistance) / (maxDistance - minDistance);
        float newVolume = Mathf.Lerp(maxVolume, minVolume, distRatio);

        // Check if approaching or receding by comparing current distance to last frame
        bool isApproaching = (distance < lastDistance);

        // If approaching, pitch moves toward maxPitch
        // If receding, pitch moves toward minPitch
        float newPitch = isApproaching
            ? Mathf.Lerp(minPitch, maxPitch, 1f - distRatio)
            : Mathf.Lerp(maxPitch, minPitch, distRatio);

        // Apply volume & pitch
        sirenAudio.volume = newVolume;
        sirenAudio.pitch = newPitch;

        // Store distance for next frame
        lastDistance = distance;
    }
}
