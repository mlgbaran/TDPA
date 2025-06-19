using System.Collections;
using UnityEngine;
using System.Reflection;
using UnityEngine.Rendering.Universal;

public class LightningController2D : MonoBehaviour
{

    [Header("Player Trigger")]
    public Transform player;    // reference to player's transform
    public float triggerX = 10; // the X-position threshold

    private bool lightningStarted = false;


    [Header("Lights to Flash")]
    public Light2D backgroundLight;
    public Light2D[] lightningLights;

    [Header("Random Strike Settings")]
    public float minStrikeInterval = 2f;
    public float maxStrikeInterval = 5f;

    [Header("Flashes Per Strike")]
    public int minFlashes = 2;
    public int maxFlashes = 5;

    [Header("Flash Duration Range")]
    public float minFlashDuration = 0.05f;
    public float maxFlashDuration = 0.2f;

    [Header("Flash Cooldown Range")]
    public float minFlashCooldown = 0.1f;
    public float maxFlashCooldown = 0.3f;

    [Header("Volumetric Range (if needed)")]
    public float minVolumetricIntensity = 0f;
    public float maxVolumetricIntensity = 1f;

    [Header("Thunder Settings")]
    public AudioSource thunderAudioSource;
    public AudioClip[] thunderClips;

    [Tooltip("Minimum delay between the lightning flash and its thunder (seconds).")]
    public float minThunderDelay = 1f;

    [Tooltip("Maximum delay between the lightning flash and its thunder (seconds).")]
    public float maxThunderDelay = 4f;

    [Tooltip("Lowest possible volume for thunder.")]
    public float minThunderVolume = 0.8f;

    [Tooltip("Highest possible volume for thunder.")]
    public float maxThunderVolume = 1.0f;

    [Tooltip("Range for random pitch variation, e.g. 0.95 ~ 1.05.")]
    public float pitchVarianceMin = 0.95f;
    public float pitchVarianceMax = 1.05f;

    private bool firstStrikeOccurred = false;

    [Header("Crows")]
    public Animator crow1Animator;
    public Animator crow2Animator;
    public CrowMovement crow1Movement;
    public CrowMovement crow2Movement;
    [Tooltip("Delay between Crow1 and Crow2 starting to fly (seconds).")]
    public float crowFlyDelay = 0.2f; // 0.2 seconds delay

    [Header("Rain")]
    public ParticleSystem rainParticleSystem;
    public AudioSource rainAudio;
    public float rainAudioLerp = 3f;
    public float rainStartDelay = 1;
    public float rainMaxVolume = 0.2f;

    // Reflection handle to Light2D's private field
    private static FieldInfo volumeIntensityField;

    // We'll store the original intensities to restore them after each flash
    private float[] originalIntensities;
    // We'll also store original volumetric values (since we can't get them from the property if it's read-only).
    // We'll fetch them via reflection at Start().
    private float[] originalVolumetrics;

    void Awake()
    {
        // Grab reflection handle
        volumeIntensityField = typeof(Light2D).GetField("m_LightVolumeIntensity",
            BindingFlags.NonPublic | BindingFlags.Instance);
    }

    void Start()
    {
        if (lightningLights == null || lightningLights.Length == 0)
        {
            Debug.LogWarning("No child lights assigned to LightningController2D.");
            return;
        }

        originalIntensities = new float[lightningLights.Length];
        originalVolumetrics = new float[lightningLights.Length];

        for (int i = 0; i < lightningLights.Length; i++)
        {
            // Store original intensity
            originalIntensities[i] = lightningLights[i].intensity;

            // Store original volumetric intensity by reflection
            if (volumeIntensityField != null)
            {
                object boxedValue = volumeIntensityField.GetValue(lightningLights[i]);
                if (boxedValue is float val)
                    originalVolumetrics[i] = val;
            }
        }

        InitializeCrowsIdle();

        //StartCoroutine(LightningRoutine());
    }

    void Update()
    {
        if (!lightningStarted && player.position.x >= triggerX)
        {
            lightningStarted = true;
            StartCoroutine(LightningRoutine());
        }
    }

    void InitializeCrowsIdle()
    {
        if (crow1Animator != null)
        {
            crow1Animator.Play("Idle1"); // Ensure Idle1 is played
        }
        if (crow2Animator != null)
        {
            crow2Animator.Play("Idle2"); // Ensure Idle2 is played
        }
    }

    IEnumerator LightningRoutine()
    {

        while (true)
        {
            // Wait random time

            if (firstStrikeOccurred == true)
            {
                float waitTime = Random.Range(minStrikeInterval, maxStrikeInterval);
                yield return new WaitForSeconds(waitTime);
            }
            else
            {
                //yield return new WaitForSeconds(3f);
            }


            // Random number of flashes
            int flashCount = Random.Range(minFlashes, maxFlashes + 1);

            for (int i = 0; i < flashCount; i++)
            {

                float flashVolumetric = Random.Range(minVolumetricIntensity, maxVolumetricIntensity);

                // Set child lights
                for (int j = 0; j < lightningLights.Length; j++)
                {

                    SetLightVolumeIntensity(lightningLights[j], flashVolumetric);

                    // Make sure volumetric is enabled if you want to see it
                    lightningLights[j].volumeIntensityEnabled = (flashVolumetric > 0f);
                }

                // (Optional) Flicker the background light
                if (backgroundLight != null)
                {
                    SetLightVolumeIntensity(backgroundLight, flashVolumetric);
                    backgroundLight.volumeIntensityEnabled = (flashVolumetric > 0f);
                }

                float currentFlashDuration = Random.Range(minFlashDuration, maxFlashDuration);

                // Stay bright
                yield return new WaitForSeconds(currentFlashDuration);

                // Revert
                for (int j = 0; j < lightningLights.Length; j++)
                {
                    lightningLights[j].intensity = originalIntensities[j];
                    SetLightVolumeIntensity(lightningLights[j], originalVolumetrics[j]);
                    lightningLights[j].volumeIntensityEnabled = (originalVolumetrics[j] > 0f);
                }

                if (backgroundLight != null)
                {
                    // Assign your real original background intensities here if needed
                    backgroundLight.intensity = 1f;
                    SetLightVolumeIntensity(backgroundLight, 0f);
                    backgroundLight.volumeIntensityEnabled = false;
                }

                float currentFlashCooldown = Random.Range(minFlashCooldown, maxFlashCooldown);

                // Pause before next flash in the same strike
                yield return new WaitForSeconds(currentFlashCooldown);

                // ----- THUNDER -----
                // We'll schedule the thunder to happen after a random delay:
                float thunderDelay = Random.Range(minThunderDelay, maxThunderDelay);

                // Pick a random thunder clip
                AudioClip chosenThunder = thunderClips[Random.Range(0, thunderClips.Length)];

                // Start a separate coroutine so it can overlap if future lightning triggers
                StartCoroutine(PlayThunderAfterDelay(chosenThunder, thunderDelay));

                // If your flash has a short duration, yield here to separate flashes
                //yield return new WaitForSeconds(0.2f);
            }

            if (!firstStrikeOccurred)
            {
                firstStrikeOccurred = true;
                StartCoroutine(TriggerCrowsToFly());
            }
        }
    }

    private IEnumerator TriggerCrowsToFly()
    {
        // Trigger Crow1 to start flying
        if (crow1Animator != null)
        {
            crow1Animator.SetTrigger("StartFly"); // Ensure "StartFly" trigger exists in Animator
            if (crow1Movement != null)
            {
                crow1Movement.StartFlying();
            }
        }

        // Wait for the specified delay before triggering Crow2
        yield return new WaitForSeconds(crowFlyDelay);

        // Trigger Crow2 to start flying
        if (crow2Animator != null)
        {
            crow2Animator.SetTrigger("StartFly"); // Ensure "StartFly" trigger exists in Animator
            if (crow2Movement != null)
            {
                crow2Movement.StartFlying();
            }
        }

        // Wait for the specified delay before triggering Crow2
        yield return new WaitForSeconds(rainStartDelay);

        StartRain();

    }

    public void StartRain()
    {
        rainParticleSystem.Play();
        rainAudio.Play();
        StartCoroutine(FadeInAudio());
    }

    private IEnumerator FadeInAudio()
    {
        float elapsedTime = 0f;
        while (elapsedTime < rainAudioLerp)
        {
            rainAudio.volume = Mathf.Lerp(0f, rainMaxVolume, elapsedTime / rainAudioLerp);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        rainAudio.volume = rainMaxVolume; // Ensure the volume is set to 1 at the end
    }

    private IEnumerator PlayThunderAfterDelay(AudioClip thunderClip, float delay)
    {
        // Wait for the specified delay (simulating thunder traveling from distance)
        yield return new WaitForSeconds(delay);

        if (thunderClip == null || thunderAudioSource == null)
            yield break;

        // Randomize pitch & volume for variation
        thunderAudioSource.pitch = Random.Range(pitchVarianceMin, pitchVarianceMax);
        thunderAudioSource.volume = Random.Range(minThunderVolume, maxThunderVolume);

        // PlayOneShot allows overlapping thunder
        thunderAudioSource.PlayOneShot(thunderClip);
    }

    void SetLightVolumeIntensity(Light2D light2D, float newValue)
    {
        if (volumeIntensityField != null && light2D != null)
        {
            volumeIntensityField.SetValue(light2D, newValue);
        }
    }

    private IEnumerator LerpLightIntensity(Light2D light, float start, float end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            light.intensity = Mathf.Lerp(start, end, t);
            yield return null;
        }
    }
}
