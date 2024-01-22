using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightFlickerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Light2D light2D;
    public int minFlickerCount = 3; // Minimum number of flickers
    public int maxFlickerCount = 4; // Maximum number of flickers
    public float minInterval = 5f; // Minimum time interval between flickers
    public float maxInterval = 6f; // Maximum time interval between flickers
    public float minFlickerDuration = 0.05f; // Minimum duration of each flicker
    public float maxFlickerDuration = 0.15f; // Maximum duration of each flicker

    private float nextFlickerTime = 0f;

    void Update()
    {
        if (Time.time >= nextFlickerTime)
        {
            nextFlickerTime = Time.time + Random.Range(minInterval, maxInterval);

            int flickerCount = Random.Range(minFlickerCount, maxFlickerCount + 1);
            StartCoroutine(FlickerLight(flickerCount));
        }
    }

    System.Collections.IEnumerator FlickerLight(int count)
    {
        for (int i = 0; i < count; i++)
        {
            float flickerDuration = Random.Range(minFlickerDuration, maxFlickerDuration);

            light2D.enabled = false;
            yield return new WaitForSeconds(flickerDuration);
            light2D.enabled = true;
            yield return new WaitForSeconds(flickerDuration);
        }
    }
}
