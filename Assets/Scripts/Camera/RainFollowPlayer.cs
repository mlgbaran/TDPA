using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainFollowPlayer : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Transform player; // The player transform


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!player)
            return;


        Vector3 sideViewPos = new Vector3(
            player.position.x,
            transform.position.y,
            transform.position.z  // keep current camera Z
        );

        // Just set the position directly, no SmoothDamp

        transform.position = sideViewPos;
        return;


    }
}
