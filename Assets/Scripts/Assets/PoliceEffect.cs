using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal; // Required for Light2D

[RequireComponent(typeof(AudioSource))]
public class PoliceEffect : MonoBehaviour
{
    [Header("Trigger Settings")]
    [Tooltip("Transform representing the trigger point.")]
    public Transform triggerPoint;

    [Header("Light Movement Points")]
    [Tooltip("Transform representing the start point of the lights.")]
    public Transform startPoint;

    [Tooltip("Transform representing the end point of the lights.")]
    public Transform endPoint;

    [Header("Lights")]
    [Tooltip("GameObject representing the blue light.")]
    public GameObject blueLight;

    [Tooltip("GameObject representing the red light.")]
    public GameObject redLight;

    [Header("Siren Settings")]
    [Tooltip("AudioSource component for the siren sound.")]
    public AudioSource sirenAudioSource;

    [Tooltip("Siren AudioClip (optional if already assigned to AudioSource).")]
    public AudioClip sirenClip;

    [Header("Effect Settings")]
    [Tooltip("Speed at which the lights move.")]
    public float lightMoveSpeed = 5f;

    [Tooltip("Interval between light flashes in seconds.")]
    public float flashInterval = 0.5f;

    [Tooltip("Maximum intensity of the lights when bright.")]
    public float brightIntensity = 1f;

    [Tooltip("Speed of the flashing effect.")]
    public float flashSpeed = 2f;

    [Tooltip("Minimum intensity of the lights when dim.")]
    public float dimIntensity = 0.2f;

    public Vector3 blueLightOffset = new Vector3(1f, 0f, 0f); // Adjust as needed

    private bool isActivated = false;
    private bool isMovingToEnd = true;
    private float flashTimer = 0f;

    void Start()
    {
        // Ensure all references are assigned
        if (triggerPoint == null)
        {
            Debug.LogError("Trigger Point is not assigned.");
        }

        if (startPoint == null || endPoint == null)
        {
            Debug.LogError("Start Point and/or End Point are not assigned.");
        }

        if (blueLight == null || redLight == null)
        {
            Debug.LogError("Blue Light and/or Red Light GameObjects are not assigned.");
        }

        if (sirenAudioSource == null)
        {
            sirenAudioSource = GetComponent<AudioSource>();
        }

        if (sirenClip != null && sirenAudioSource.clip == null)
        {
            sirenAudioSource.clip = sirenClip;
            sirenAudioSource.loop = false;
        }

        // Initialize lights at start point
        blueLight.transform.position = startPoint.position + blueLightOffset;
        redLight.transform.position = startPoint.position;

        // Ensure lights are active
        blueLight.SetActive(false);
        redLight.SetActive(false);
    }

    void Update()
    {
        if (!isActivated)
        {
            CheckPlayerPosition();
        }
        else
        {
            MoveLights();
            HandleFlashing();
        }
    }

    /// <summary>
    /// Checks if the player has crossed the trigger point's X position.
    /// </summary>
    void CheckPlayerPosition()
    {
        // Find the player GameObject (Assuming it has the tag "Player")
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player GameObject with tag 'Player' not found.");
            return;
        }

        float playerX = player.transform.position.x;
        float triggerX = triggerPoint.position.x;

        if (playerX > triggerX)
        {
            ActivatePoliceEffect();
        }
    }

    /// <summary>
    /// Activates the police effect: starts moving lights, flashing, and plays siren.
    /// </summary>
    void ActivatePoliceEffect()
    {
        isActivated = true;

        // Start moving lights from start point
        blueLight.transform.position = startPoint.position;
        redLight.transform.position = startPoint.position;

        blueLight.SetActive(true);
        redLight.SetActive(true);

        // Play siren sound
        if (sirenAudioSource != null && sirenClip != null)
        {
            sirenAudioSource.Play();
        }
    }

    /// <summary>
    /// Moves the lights between start and end points.
    /// </summary>
    void MoveLights()
    {
        float step = lightMoveSpeed * Time.deltaTime;

        // Determine target position
        Vector3 redTarget = isMovingToEnd ? endPoint.position : startPoint.position;

        //Vector3 blueTarget = redTarget + blueLightOffset;

        // Move blue light
        redLight.transform.position = Vector3.MoveTowards(redLight.transform.position, redTarget, step);
        // Move blue light towards its offset target
        blueLight.transform.position = redLight.transform.position + blueLightOffset;

        // Check if reached target
        //if (Vector3.Distance(blueLight.transform.position, targetPos) < 0.001f)
        //{
        //    isMovingToEnd = !isMovingToEnd; // Reverse direction
        //}
    }

    /// <summary>
    /// Handles the flashing of lights by toggling their active state.
    /// </summary>
    private void HandleFlashing()
    {
        // Calculate oscillating value between 0 and 1 using sine wave
        float t = (Mathf.Sin(Time.time * flashSpeed) + 1f) / 2f;

        // Smoothly interpolate intensity between dim and bright
        float currentIntensity = Mathf.Lerp(dimIntensity, brightIntensity, t);

        // Apply the calculated intensity to both lights
        if (blueLight != null)
            blueLight.GetComponent<Light2D>().intensity = currentIntensity;

        if (redLight != null)
            redLight.GetComponent<Light2D>().intensity = currentIntensity;
    }


    /// <summary>
    /// Visualizes the trigger point, start point, and end point with Gizmos.
    /// </summary>
    void OnDrawGizmos()
    {
        // Draw Trigger Point
        if (triggerPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(triggerPoint.position, 0.05f);
            Gizmos.DrawLine(triggerPoint.position + Vector3.up * 1f, triggerPoint.position + Vector3.down * 1f);
        }

        // Draw Start Point
        if (startPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(startPoint.position, 0.05f);
            //Gizmos.DrawLine(startPoint.position + Vector3.right * 1f, startPoint.position + Vector3.left * 1f);
        }

        // Draw End Point
        if (endPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(endPoint.position, 0.05f);
            //Gizmos.DrawLine(endPoint.position + Vector3.right * 1f, endPoint.position + Vector3.left * 1f);
        }
    }
}
