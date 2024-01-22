using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class NPCRotation : MonoBehaviour
{


    public bool isLookingToSth = false;

    [HideInInspector] public float myDirectionX;

    [HideInInspector] public float myDirectionY;

    public Animator animator;
    public bool isEquipped;
    public GameObject lookPosition = null;

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


    //set this with an All-NPC Controller maybe ?
    public float smallDegree = 20.5f;
    float tansmall;
    float tanbig;


    //Big problem
    public ShoulderScript shoulderScript;

    void Awake()
    {
        //set the is looking to false, assign it later.

        //set it true just for debugging
        isLookingToSth = true;


    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //so that the view angles can be set on runtime




        if (isLookingToSth)
        {

            animator.SetBool("IsLooking", true);

            tansmall = Mathf.Tan(smallDegree * Mathf.Deg2Rad);

            tanbig = Mathf.Tan((90f - smallDegree) * Mathf.Deg2Rad);

            LookAt(lookPosition.transform.position);

        }








    }


    void LookAt(UnityEngine.Vector3 target)
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

        if (dir.x > 0 && abstanDir < tansmall)          //RIGHT
        {
            if (shoulderScript != null && isEquipped)
            {
                shoulderScript.SetArmZ(0.41f, true);
                shoulderScript.SetWidth(2.96f);
                shoulderScript.setZlevel(-0.4f);
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
                shoulderScript.SetArmZ(0, false);
                shoulderScript.SetArmZ(0, true);
                shoulderScript.SetWidth(4.84f);
                shoulderScript.setZlevel(-0.4f);

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
                shoulderScript.SetArmZ(0, false);
                shoulderScript.SetArmZ(0, true);
                shoulderScript.SetWidth(6.37f);
                shoulderScript.setZlevel(-0.4f);
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
                shoulderScript.SetArmZ(0, false);
                shoulderScript.SetArmZ(0, true);
                shoulderScript.SetWidth(4.84f);
                shoulderScript.setZlevel(-0.4f);
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
                shoulderScript.SetArmZ(0.41f, false);
                shoulderScript.SetWidth(2.96f);
                shoulderScript.setZlevel(-0.4f);
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
                shoulderScript.SetArmZ(0, false);
                shoulderScript.SetArmZ(0, true);
                shoulderScript.SetWidth(4.84f);
                shoulderScript.setZlevel(0.01f);
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
                shoulderScript.SetArmZ(0, false);
                shoulderScript.SetArmZ(0, true);
                shoulderScript.SetWidth(6.37f);
                shoulderScript.setZlevel(0.01f);
            }
            myDirectionX = 1;
            myDirectionY = 0;
            animator.SetFloat("DirectionY", 1);
            animator.SetFloat("DirectionX", 0);

        }
        else if (dir.x > 0 && dir.y > 0 && abstanDir > tansmall) //Right UP
        {
            if (shoulderScript != null && isEquipped)
            {
                shoulderScript.SetArmZ(0, false);
                shoulderScript.SetArmZ(0, true);
                shoulderScript.SetWidth(4.84f);
                shoulderScript.setZlevel(0.01f);
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

    }

}
