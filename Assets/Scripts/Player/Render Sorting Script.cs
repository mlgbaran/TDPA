using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderSortingScript : MonoBehaviour
{
    private IsoObjectSorterScript[] sorters;

    public float radius = 0.4f; // Public radius field to determine the color of the line

    void Start()
    {
        // Find all GameObjects with the IsoObjectSorterScript
        sorters = FindObjectsOfType<IsoObjectSorterScript>();

        Debug.Log("Found " + sorters.Length + " objects with IsoObjectSorterScript.");
    }
    // Update is called once per frame
    void OnDrawGizmos()
    {
        if (sorters == null || sorters.Length == 0)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);

        // Draw a line from the player to each object with the IsoObjectSorterScript
        foreach (var sorter in sorters)
        {

            Vector2 midpoint = sorter.GetMidpoint();
            Vector2 objectPosition = sorter.GetObjectPosition();
            //Debug.Log("Drawing line to object at position: " + objectPosition);

            // Determine the distance between the player and the object
            float distance = Vector2.Distance(transform.position, objectPosition);

            // Set the line color based on the distance
            if (distance <= radius)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }

            // Draw the line
            Gizmos.DrawLine(transform.position, objectPosition + midpoint);
        }
    }
}
