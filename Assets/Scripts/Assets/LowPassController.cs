using UnityEngine;
using UnityEngine.Audio;

public class LowPassController : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioMixer silhouetteRainMixer; // Assign in Inspector
    public string exposedParam = "MasterLowPassCutoff"; // Name of the exposed parameter

    [Header("Cutoff Frequencies")]
    public float cutoffLow = 1175f;
    public float cutoffHigh = 12000f;
    public float transitionDuration = 1.0f; // Duration of the transition in seconds

    [Header("Position Settings")]
    public Transform playerTransform; // Assign the player in Inspector
    public float switchXPosition = 0f; // Set your target x position

    private bool isAbove = false;
    private float targetCutoff;

    private void Start()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is not assigned.");
        }

        // Initialize the cutoff frequency based on the player's initial position
        UpdateCutoffBasedOnPosition(true);
    }

    private void Update()
    {
        UpdateCutoffBasedOnPosition(false);
    }

    private void UpdateCutoffBasedOnPosition(bool forceUpdate)
    {
        if (playerTransform == null) return;

        bool currentlyAbove = playerTransform.position.x > switchXPosition;

        if (currentlyAbove != isAbove || forceUpdate)
        {
            isAbove = currentlyAbove;
            targetCutoff = isAbove ? cutoffHigh : cutoffLow;
            StopAllCoroutines(); // Stop any ongoing transitions
            StartCoroutine(TransitionCutoff(targetCutoff));
        }
    }

    private System.Collections.IEnumerator TransitionCutoff(float newCutoff)
    {
        float currentCutoff;
        silhouetteRainMixer.GetFloat(exposedParam, out currentCutoff);
        float elapsed = 0f;
        float startCutoff = currentCutoff;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionDuration);
            float interpolatedCutoff = Mathf.Lerp(startCutoff, newCutoff, t);
            silhouetteRainMixer.SetFloat(exposedParam, interpolatedCutoff);
            yield return null;
        }

        silhouetteRainMixer.SetFloat(exposedParam, newCutoff); // Ensure exact final value
    }
}
