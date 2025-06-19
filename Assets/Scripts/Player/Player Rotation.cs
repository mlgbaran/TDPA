using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerRotation : MonoBehaviour
{

    [SerializeField] Camera cam;

    public bool sideView = false;

    public float myDirectionX;

    public float myDirectionY;

    public PlayerMovement playerMovement;

    public Animator animator;

    public float animateThreshold;

    public bool isEquipped;

    UnityEngine.Vector3 mouseposition;

    public bool lookAtLookDirection;
    public GameObject lookDirection;
    Color color;

    UnityEngine.Vector3 ustsag;
    UnityEngine.Vector3 sagust;
    UnityEngine.Vector3 sagalt;
    UnityEngine.Vector3 altsag;
    UnityEngine.Vector3 altsol;
    UnityEngine.Vector3 solalt;
    UnityEngine.Vector3 solust;
    UnityEngine.Vector3 ustsol;

    UnityEngine.Vector3 dir;

    public bool debugmode = false;

    public float smallDegree = 20.5f;

    float tansmall;

    float tanbig;

    bool isTargetFarEnough = false;

    public ShoulderScript shoulderScript;

    [HideInInspector] public ArmScript armScript;

    public float xShoulderShift = 0;

    public float yShoulderShift = 0;

    public bool dialogueMode;

    float movementX;

    float movementY;

    // Start is called before the first frame update
    void Start()
    {
        isTargetFarEnough = false;


    }

    // Update is called once per frame
    void Update()
    {
        //so that the view angles can be set on runtime

        tansmall = Mathf.Tan(smallDegree * Mathf.Deg2Rad);

        tanbig = Mathf.Tan((90f - smallDegree) * Mathf.Deg2Rad);


        if (lookAtLookDirection == false)
        {
            //if player's/NPC's direction is going to be determined by the mouse

            mouseposition = cam.ScreenToWorldPoint(Input.mousePosition);

            isTargetFarEnough = CheckTargetDistance(mouseposition);

            if (isTargetFarEnough)
            {
                LookAt(mouseposition);
            }


        }
        else if (lookAtLookDirection == true)
        {
            //if player's/NPC's direction is going to be determined by the LookDirection GameObject's position

            isTargetFarEnough = CheckTargetDistance(lookDirection.transform.position);

            if (isTargetFarEnough)
            {
                LookAt(lookDirection.transform.position);
            }

        }






    }


    void LookAt(UnityEngine.Vector3 target)
    {


        if (sideView == false)
        {
            target.z = 10;




            //Debug.Log(animator.avatar);



            //topright topleft vs

            ustsag = new UnityEngine.Vector3(transform.position.x + tansmall, transform.position.y + 1, transform.position.z);

            sagust = new UnityEngine.Vector3(transform.position.x + 1, transform.position.y + tansmall, transform.position.z);

            sagalt = new UnityEngine.Vector3(transform.position.x + 1, transform.position.y - tansmall, transform.position.z);

            altsag = new UnityEngine.Vector3(transform.position.x + tansmall, transform.position.y - 1, transform.position.z);

            altsol = new UnityEngine.Vector3(transform.position.x - tansmall, transform.position.y - 1, transform.position.z);

            solalt = new UnityEngine.Vector3(transform.position.x - 1, transform.position.y - tansmall, transform.position.z);

            solust = new UnityEngine.Vector3(transform.position.x - 1, transform.position.y + tansmall, transform.position.z);

            ustsol = new UnityEngine.Vector3(transform.position.x - tansmall, transform.position.y + 1, transform.position.z);



            dir = (target - transform.position).normalized;

            dir.z = transform.position.z;


            float tanDir = dir.y / dir.x;

            //declaring tansmall and tan 60 values


            float abstanDir = Math.Abs(tanDir);


            //WTF




            //6.37 for up/down

            //4.84 for right/left up/down

            //2.96 for right/left


            //0.01: behind the head, -0.2: in front of the head.



            //where is the player going ?
            movementX = playerMovement.movementX;
            movementY = playerMovement.movementY;


            //CANCELED-----------------------------------------------------------------------------------------------------------------------------

            //When going towards one direction, if the player looks towards the opposite direction, player's movement speed will be reduced (halved)

            //if aiming (holding right mouse button)

            //CANCELED-----------------------------------------------------------------------------------------------------------------------------


            if (dir.x > 0 && abstanDir < tansmall)          //RIGHT
            {
                if (shoulderScript != null && isEquipped)
                {
                    shoulderScript.SetWidth(2.96f);


                    //shoulderScript.SetPos(xShoulderShift, yShoulderShift);

                }
                myDirectionX = 1;
                myDirectionY = 0;
                animator.SetFloat("DirectionX", 1);
                animator.SetFloat("DirectionY", 0);



            }
            else if (dir.x > 0 && dir.y < 0 && abstanDir < tanbig) //RIGHT DOWN
            {

                if (shoulderScript != null && isEquipped)
                {
                    shoulderScript.SetWidth(4.84f);


                }
                myDirectionX = 1;
                myDirectionY = -1;
                animator.SetFloat("DirectionX", 1);
                animator.SetFloat("DirectionY", -1);

            }

            else if (dir.y < 0 && abstanDir > tanbig) //DOWN
            {

                if (shoulderScript != null && isEquipped)
                {
                    shoulderScript.SetWidth(6.37f);

                }
                myDirectionX = 0;
                myDirectionY = -1;
                animator.SetFloat("DirectionX", 0);
                animator.SetFloat("DirectionY", -1);

            }
            else if (dir.x < 0 && dir.y < 0 && abstanDir > tansmall) //LEFT DOWN
            {

                if (shoulderScript != null && isEquipped)
                {
                    shoulderScript.SetWidth(4.84f);

                }
                myDirectionX = -1;
                myDirectionY = -1;
                animator.SetFloat("DirectionX", -1);
                animator.SetFloat("DirectionY", -1);

            }
            else if (dir.x < 0 && abstanDir < tansmall)          //LEFT
            {

                if (shoulderScript != null && isEquipped)
                {
                    shoulderScript.SetWidth(2.96f);

                }
                myDirectionX = -1;
                myDirectionY = 0;
                animator.SetFloat("DirectionX", -1);
                animator.SetFloat("DirectionY", 0);


            }
            else if (dir.x < 0 && dir.y > 0 && abstanDir < tanbig) //LEFT UP
            {
                if (shoulderScript != null && isEquipped)
                {
                    shoulderScript.SetWidth(4.84f);

                }
                myDirectionX = -1;
                myDirectionY = 1;
                animator.SetFloat("DirectionX", -1);
                animator.SetFloat("DirectionY", 1);

            }
            else if (dir.y > 0 && abstanDir > tanbig) //UP
            {
                if (shoulderScript != null && isEquipped)
                {
                    shoulderScript.SetWidth(6.37f);

                }
                myDirectionX = 0;
                myDirectionY = 1;
                animator.SetFloat("DirectionX", 0);
                animator.SetFloat("DirectionY", 1);

            }
            else if (dir.x > 0 && dir.y > 0 && abstanDir > tansmall) //Right UP
            {
                if (shoulderScript != null && isEquipped)
                {
                    shoulderScript.SetWidth(4.84f);

                }
                myDirectionX = 1;
                myDirectionY = 1;
                animator.SetFloat("DirectionX", 1);
                animator.SetFloat("DirectionY", 1);


            }

            if (debugmode == true)
            {


                Debug.DrawLine(transform.position, ustsag, Color.green);

                Debug.DrawLine(transform.position, sagust, Color.green);

                Debug.DrawLine(transform.position, sagalt, Color.green);

                Debug.DrawLine(transform.position, altsag, Color.green);

                Debug.DrawLine(transform.position, altsol, Color.green);

                Debug.DrawLine(transform.position, solalt, Color.green);

                Debug.DrawLine(transform.position, solust, Color.green);

                Debug.DrawLine(transform.position, ustsol, Color.green);

                //to mouse pos

                Debug.DrawLine(transform.position, target, Color.red);

            }

            bool isBehindPlayer = (myDirectionX == 0 && myDirectionY == 1) ||  // Up
                              (myDirectionX == -1 && myDirectionY == 1) || // Left-Up
                              (myDirectionX == 1 && myDirectionY == 1);   // Right-Up


            shoulderScript.UpdateArmSortingOrder(isBehindPlayer);
        }
        else    //if side view
        {
            dir = (target - transform.position).normalized;

            dir.z = transform.position.z;

            if (dir.x < 0)          //LEFT
            {

                if (shoulderScript != null && isEquipped)
                {
                    shoulderScript.SetWidth(2.96f);

                }
                myDirectionX = -1;
                myDirectionY = 0;
                animator.SetFloat("DirectionX", -1);
                animator.SetFloat("DirectionY", 0);


            }
            else if (dir.x > 0)
            {
                if (shoulderScript != null && isEquipped)
                {
                    shoulderScript.SetWidth(2.96f);

                }
                myDirectionX = 1;
                myDirectionY = 0;
                animator.SetFloat("DirectionX", 1);
                animator.SetFloat("DirectionY", 0);
            }

            shoulderScript.UpdateArmSortingOrder(false);
        }



    }




    bool CheckTargetDistance(UnityEngine.Vector3 target)
    {

        //if the player is equipped, turn to the mouse's direction even if the mouse is closer than the threshold
        if (isEquipped)
        {

            return true;
        }
        //Checking if the Mouse is far enough

        bool result = false;

        if (Math.Abs(target.x - transform.position.x) > animateThreshold || Math.Abs(target.y - transform.position.y) > animateThreshold)
        {

            //animator.SetBool("IsLooking", true);
            result = true;
        }
        else
        {
            //animator.SetBool("IsLooking", false);
            result = false;
        }


        return result;

    }
}
