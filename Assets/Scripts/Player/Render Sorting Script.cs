using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderSortingScript : MonoBehaviour
{
    private IsoObjectSorterScript[] sorters;
    private SpriteRenderer playerRenderer;

    public float radius = 0.4f; // Radius to detect nearby assets

    public bool debugMode = false;


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

        foreach (var sorter in sorters)
        {
            // Skip assets outside the player's radius
            if (Vector2.Distance(transform.position, sorter.GetObjectPosition()) > radius)
                continue;

            Vector2 assetPos = sorter.GetObjectPosition();
            Vector2 playerPos = new Vector2(transform.position.x, transform.position.y - 0.1f);

            // Calculate world space points for diagonal line
            Vector2 pointA = assetPos + sorter.pointA; // Bottom-left corner
            Vector2 pointB = assetPos + sorter.pointB; // Top-right corner

            // Compute cross product to determine player's position relative to the line
            float crossProduct = (pointB.x - pointA.x) * (playerPos.y - pointA.y) -
                                 (pointB.y - pointA.y) * (playerPos.x - pointA.x);

            // Adjust sorting order
            if (crossProduct < 0) // Player is below the line
            {
                playerRenderer.sortingOrder = baseOrder + 1; // Render in front
            }
            else // Player is above the line
            {
                playerRenderer.sortingOrder = baseOrder - 1; // Render behind
            }
        }
    }





    void OnDrawGizmos()
    {
        if (!debugMode) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);

        if (sorters == null || sorters.Length == 0) return;

        Vector2 playerPos = new Vector2(transform.position.x, transform.position.y - 0.1f);

        Gizmos.DrawSphere(playerPos, 0.005f);

        foreach (var sorter in sorters)
        {
            Vector2 assetPos = sorter.GetObjectPosition();

            // Skip assets outside the radius
            if (Vector2.Distance(playerPos, assetPos) > radius)
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
