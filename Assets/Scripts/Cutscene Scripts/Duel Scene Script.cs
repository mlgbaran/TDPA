using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class DuelSceneScript : MonoBehaviour
{

    public Animator playerAnimator;
    public PlayerMovement playerMovement;

    private float originalMoveSpeed;




    public Transform mainCameraTransform;  // Assign the Camera's Transform
    public Transform player;  // Reference to the player
    public Transform startPoint;  // The point where ortho size = 0.7
    public Transform endPoint;    // The point where ortho size = 0.8

    public Vector3 minScale = new Vector3(0.45f, 0.45f, 0.45f); // Scale at startPoint
    public Vector3 maxScale = new Vector3(0.5f, 0.5f, 0.5f); // Scale at endPoint
    public float smoothSpeed = 5f; // Speed of interpolationyea

    private bool playerInZone = false;


    void Start()
    {

        originalMoveSpeed = playerMovement.moveSpeed;
        playerInZone = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerAnimator.SetBool("shouldWalk", true);
            playerMovement.moveSpeed = 0.20f;
            playerInZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerAnimator.SetBool("shouldWalk", false);
            playerMovement.moveSpeed = originalMoveSpeed;
            playerInZone = false;
        }
    }


    private void Update()
    {
        if (mainCameraTransform == null || player == null || startPoint == null || endPoint == null)
            return;

        Vector3 targetScale = minScale;

        if (playerInZone)
        {
            // Calculate interpolation factor (0 to 1) based on player's position between start and end
            float t = Mathf.InverseLerp(startPoint.position.x, endPoint.position.x, player.position.x);
            targetScale = Vector3.Lerp(minScale, maxScale, t);

            //Debug.Log(targetScale);

            mainCameraTransform.localScale = new Vector3(Mathf.Lerp(mainCameraTransform.localScale.x, targetScale.x, Time.deltaTime * smoothSpeed), Mathf.Lerp(mainCameraTransform.localScale.y, targetScale.y, Time.deltaTime * smoothSpeed), Mathf.Lerp(mainCameraTransform.localScale.z, targetScale.z, Time.deltaTime * smoothSpeed));
        }

        // Smoothly interpolate the camera size

    }
}
