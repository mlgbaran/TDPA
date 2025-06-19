using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DopplerEffect : MonoBehaviour
{

    public Transform playerTransform;

    public AudioSource audioSource;

    public Transform startPos;

    public Transform endPos;

    private float distanceToPlayer;
    private float maxDistance;

    private float minDistance = 0;
    // Start is called before the first frame update
    void Awake()
    {
        distanceToPlayer = transform.position.x - playerTransform.position.x;

        float startX = startPos.position.x;
        float endX = endPos.position.x;

        // Calculate the distances from the current object to the start and end positions
        float distanceToStart = Mathf.Abs(transform.position.x - startX);
        float distanceToEnd = Mathf.Abs(transform.position.x - endX);

        // Determine the furthest distance
        maxDistance = distanceToEnd;




    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = math.abs(transform.position.x - playerTransform.position.x);



        // Normalize the distanceToPlayer between 0 and 1
        float normalizedDistance = Mathf.Clamp01(distanceToPlayer / maxDistance);

        audioSource.volume = 1f - normalizedDistance;
    }
}
