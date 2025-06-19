using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer2 : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Transform player; // The player transform

    [Header("Side View")]
    public bool sideView = false;
    public float sideViewOffsetX = 0;
    public float sideViewOffsetY = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!player)
            return;

        if (sideView)
        {
            Vector3 sideViewPos = new Vector3(
                player.position.x + sideViewOffsetX,
                player.position.y + sideViewOffsetY,
                transform.position.z  // keep current camera Z
            );

            // Just set the position directly, no SmoothDamp

            transform.position = sideViewPos;
            return;
        }

    }
}
