using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Camera cam;
    [SerializeField] Transform player;
    [SerializeField] float threshold;



    // Update is called once per frame
    void Update()
    {
        UnityEngine.Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        UnityEngine.Vector3 targetPos = (player.position + mousePos) / 2f;


        targetPos.x = Mathf.Clamp(targetPos.x, -threshold + player.position.x, threshold + player.position.x);
        targetPos.y = Mathf.Clamp(targetPos.y, -threshold + player.position.y, threshold + player.position.y);

        this.transform.position = targetPos;


    }
}
