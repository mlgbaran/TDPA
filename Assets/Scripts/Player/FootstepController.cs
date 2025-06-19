using UnityEngine;

public class FootstepController : MonoBehaviour
{
    [Header("Footstep Audio Clips")]
    [Tooltip("Audio clips for solid surfaces.")]
    public AudioClip[] solidFootsteps;

    [Tooltip("Audio clips for wet surfaces.")]
    public AudioClip[] wetFootsteps;

    [Header("Footstep Settings")]
    [Tooltip("Time interval between footsteps when walking.")]
    public float walkStepInterval = 0.5f;

    [Tooltip("Time interval between footsteps when running.")]
    public float runStepInterval = 0.3f;

    [Tooltip("X position boundary to switch footstep sounds.")]
    public float boundaryX = 0f;

    [Header("Randomization Settings")]
    [Tooltip("Minimum pitch variation.")]
    public float minPitch = 0.95f;

    [Tooltip("Maximum pitch variation.")]
    public float maxPitch = 1.05f;

    [Tooltip("Minimum volume variation.")]
    public float minVolume = 0.8f;

    [Tooltip("Maximum volume variation.")]
    public float maxVolume = 1.0f;

    [Header("Movement Detection")]
    [Tooltip("Reference to the Rigidbody2D component.")]
    public PlayerMovement playerMovement;

    [Header("Audio Source")]
    [Tooltip("Reference to the AudioSource on the 'footsteps' child.")]
    public AudioSource footstepAudioSource;

    private float stepTimer;
    private bool isRunning;

    void Awake()
    {
        // If the AudioSource is not assigned in the Inspector, attempt to find it on the child.
        if (footstepAudioSource == null)
        {
            Transform footstepsTransform = transform.Find("footsteps");
            if (footstepsTransform != null)
            {
                footstepAudioSource = footstepsTransform.GetComponent<AudioSource>();
                if (footstepAudioSource == null)
                {
                    Debug.LogError("AudioSource component missing on the 'footsteps' child GameObject.");
                }
            }
            else
            {
                Debug.LogError("Child GameObject named 'footsteps' not found.");
            }
        }

    }

    void Update()
    {
        // Determine if the character is moving
        float speed = GetCharacterSpeed();

        bool moving = speed > 0f; // Adjust threshold as needed
        isRunning = speed > 5f;      // Define your own speed threshold for running

        if (moving)
        {
            stepTimer -= Time.deltaTime;

            float currentInterval = isRunning ? runStepInterval : walkStepInterval;

            if (stepTimer <= 0f)
            {
                PlayFootstepSound();
                stepTimer = currentInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    /// <summary>
    /// Retrieves the character's current speed.
    /// </summary>
    /// <returns>Current speed of the character.</returns>
    private float GetCharacterSpeed()
    {
        if (playerMovement != null)
        {
            return playerMovement.calculatedSpeed;
        }

        // Placeholder if no Rigidbody2D is used
        return 0f;
    }

    /// <summary>
    /// Plays a footstep sound based on the current position.
    /// </summary>
    private void PlayFootstepSound()
    {
        if (footstepAudioSource == null)
        {
            Debug.LogWarning("No AudioSource assigned for footstep sounds.");
            return;
        }

        // Determine current surface based on X position
        string currentSurface = transform.position.x > boundaryX ? "Wet" : "Solid";

        // Select appropriate footstep clips
        AudioClip[] footstepClips = currentSurface == "Wet" ? wetFootsteps : solidFootsteps;

        if (footstepClips.Length == 0)
        {
            Debug.LogWarning($"No footstep clips assigned for {currentSurface} surface.");
            return;
        }

        // Select a random clip
        int index = Random.Range(0, footstepClips.Length);
        AudioClip clip = footstepClips[index];

        // Apply random pitch and volume
        footstepAudioSource.pitch = Random.Range(minPitch, maxPitch);
        footstepAudioSource.volume = Random.Range(minVolume, maxVolume);

        // Play the clip
        footstepAudioSource.PlayOneShot(clip);
    }

    /// <summary>
    /// Visualize the boundary in the editor.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(boundaryX, -10f, 0f), new Vector3(boundaryX, 10f, 0f));
    }
}
