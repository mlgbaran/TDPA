using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using UnityEngine;

public class AudioFadeScript : MonoBehaviour
{

    public Transform playerListener;

    private AudioSource audioSource;

    public float radius = 0.8f;

    public bool isDebugMode = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0f;
    }

    // Update is called once per frame
    void Update()
{
    UnityEngine.Vector2 distance = playerListener.position - transform.position;

    if (distance.magnitude <= radius)
    {
        // Adjust the exponent value to control the rate of volume decrease
        float exponent = 2.0f; // You can adjust this value to suit your needs

        // Calculate the volume based on an exponential curve
        float volume = Mathf.Pow(1.0f - distance.magnitude / radius, exponent);

        // Ensure the volume stays between 0 and 1
        volume = Mathf.Clamp01(volume);

        audioSource.volume = volume;
    }
    else
    {
        // If the distance is greater than the radius, set volume to 0
        audioSource.volume = 0f;
    }
}

    void OnDrawGizmos()
    {
        if (isDebugMode)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
