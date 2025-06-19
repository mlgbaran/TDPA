using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderSortingScript : MonoBehaviour
{
    private IsoObjectSorterScript[] sorters;
    private SpriteRenderer playerRenderer;

    public Vector2 detectionSize = new Vector2(1.0f, 1.5f); // Width and Height of the rectangle
    public Vector2 detectionOffset = new Vector2(0f, -0.1f); // X and Y offset for the rectangle
    public bool debugMode = false;

    public LayerMask detectionLayer; // LayerMask for objects to check (e.g., assets and NPCs)


    bool IsWithinRectangle(Vector2 playerPos, Vector2 assetPos)
    {
        // Calculate the detection rectangle's center based on the player position and offset
        Vector2 rectCenter = playerPos + detectionOffset;

        // Calculate the player's rectangular bounds
        Vector2 rectMin = rectCenter - detectionSize / 2;
        Vector2 rectMax = rectCenter + detectionSize / 2;

        // Check if the asset's position is within the rectangle
        return assetPos.x >= rectMin.x && assetPos.x <= rectMax.x &&
               assetPos.y >= rectMin.y && assetPos.y <= rectMax.y;
    }

    bool IsOverlapping(Renderer objectRenderer)
    {
        // Check if the player's bounds intersect with the object's bounds
        return playerRenderer.bounds.Intersects(objectRenderer.bounds);
    }

    void Start()
    {
        sorters = FindObjectsOfType<IsoObjectSorterScript>();
        playerRenderer = GetComponent<SpriteRenderer>();
    }


    // Update is called once per frame
    void Update()
    {
        UpdateSortingOrder();
    }


    void UpdateSortingOrder()
    {
        int baseOrder = 1; // Default sorting order for the player
        bool isSorted = false; // Tracks whether the player's sorting order is updated

        // The player's position is their feet (unadjusted)
        Vector2 playerPos = new Vector2(transform.position.x, transform.position.y - 0.1f);

        foreach (var sorter in sorters)
        {
            Vector2 assetPos = sorter.GetObjectPosition();

            // Skip assets outside the player's detection rectangle
            if (!IsWithinRectangle(playerPos, assetPos))
                continue;

            // Check if the player overlaps with the asset's bounding box
            Renderer assetRenderer = sorter.GetComponent<Renderer>();
            if (assetRenderer == null || !IsOverlapping(assetRenderer))
                continue;

            // Calculate world space points for diagonal line
            Vector2 pointA = assetPos + sorter.pointA; // Bottom-left corner
            Vector2 pointB = assetPos + sorter.pointB; // Top-right corner

            // Compute cross product to determine player's position relative to the line
            float crossProduct = (pointB.x - pointA.x) * (playerPos.y - pointA.y) -
                                 (pointB.y - pointA.y) * (playerPos.x - pointA.x);

            // Adjust sorting order based on relative position
            if (crossProduct < 0) // Player's feet are below the line
            {
                playerRenderer.sortingOrder = baseOrder + 1; // Render in front
                isSorted = true;
            }
            else // Player's feet are above the line
            {
                playerRenderer.sortingOrder = baseOrder - 1; // Render behind
                isSorted = true;
            }

            // Debug: Log overlapping assets
            Debug.Log($"Overlapping with: {sorter.gameObject.name}");
        }

        // Fallback to default sorting if no objects affect the player
        if (!isSorted)
        {
            playerRenderer.sortingOrder = baseOrder;
        }
    }





    void OnDrawGizmos()
    {
        if (!debugMode) return;

        // Draw the detection rectangle
        Vector2 playerPos = new Vector2(transform.position.x, transform.position.y - 0.1f);
        Vector2 rectCenter = playerPos + detectionOffset;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(rectCenter, detectionSize);

        if (sorters == null || sorters.Length == 0) return;

        foreach (var sorter in sorters)
        {
            Vector2 assetPos = sorter.GetObjectPosition();

            // Skip assets outside the rectangle
            if (!IsWithinRectangle(playerPos, assetPos))
                continue;

            // Draw diagonal line of the asset
            Vector2 pointA = assetPos + sorter.pointA;
            Vector2 pointB = assetPos + sorter.pointB;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(pointA, pointB);

            // Compute cross product for debugging
            float crossProduct = (pointB.x - pointA.x) * (playerPos.y - pointA.y) -
                                 (pointB.y - pointA.y) * (playerPos.x - pointA.x);

            // Color-code the asset based on relative position
            Gizmos.color = crossProduct < 0 ? Color.green : Color.red;
            Gizmos.DrawSphere(assetPos, 0.05f);
        }
    }
}
