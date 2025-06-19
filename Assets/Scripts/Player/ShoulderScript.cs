using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class ShoulderScript : MonoBehaviour
{
    public float width;

    public float zlevel;

    private Transform leftarm;

    private Transform rightarm;

    private Transform shouldercenter;

    private float leftx;

    private float rightx;

    private float playerZ;

    private Vector3 mouse_pos;

    private PlayerRotation playerRotation;

    public Transform shootingArmsReturnPoint;

    public float xOffset;

    public float yOffset;

    public SpriteRenderer characterRenderer;




    public void UpdateArmSortingOrder(bool isBehindPlayer)
    {
        // Iterate through all arm pixel renderers
        foreach (Transform arm in transform) // Iterate over left arm and right arm
        {
            foreach (var pixel in arm.GetComponentsInChildren<SpriteRenderer>()) // Get "One Pixel" renderers
            {
                if (isBehindPlayer)
                {
                    pixel.sortingOrder = characterRenderer.sortingOrder - 1; // Render behind the player
                }
                else
                {
                    pixel.sortingOrder = characterRenderer.sortingOrder + 1; // Render in front of the player
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        leftarm = transform.GetChild(0);

        rightarm = transform.GetChild(1);

        shouldercenter = transform.GetChild(2);

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 leftArmNewPosition = transform.position - transform.right * width / 200;
        Vector3 rightArmNewPosition = transform.position + transform.right * width / 200;

        // Set the positions of the left and right arms.
        leftarm.position = leftArmNewPosition;
        rightarm.position = rightArmNewPosition;

        Vector3 newXposition = shootingArmsReturnPoint.position + transform.right * xOffset / 200;

        transform.position = newXposition;


    }


    public void SetWidth(float value)
    {
        //Debug.Log("Changed Width");
        width = value;
        //Debug.Log(width);

    }

    public void SetPos(float x = 0, float y = 0)
    {
        if (x == 0 && y == 0)
        {
            return;

        }



        if (x != 0)
        {
            //change Xpos
            xOffset = x;
        }
        if (y != 0)
        {
            //change Ypos
            yOffset = y;

        }

    }



}
