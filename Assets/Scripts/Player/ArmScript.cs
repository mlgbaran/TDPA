using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class ArmScript : MonoBehaviour
{

    public float offset;

    public bool isLeft;

    private float unitWidth;

    private Transform firstChild;
    private Transform theparent;

    private Transform thechild;

    private float childcount;

    private Transform gunhold;

    private Vector3 gunpointposition;

    public Transform gunpoint;

    private Transform dummy;

    private ShoulderScript shoulderScript;

    public Color shoulderColor;

    public Color armColor;

    public Color handColor;

    public Transform theHand;


    void Awake()
    {

        Transform prnt = transform.parent.root;

        Renderer renderer = prnt.GetComponent<SpriteRenderer>();

        Material parentMaterial = renderer.material;

        Texture charSkin = parentMaterial.GetTexture("_CharSkin");

        if (isLeft)  //if left arm, then get the pixel colors from the left arm.
        {
            shoulderColor = ((Texture2D)charSkin).GetPixel(1, 30 - 14);   //substracting from 32 because getPixel uses UV coordinates => lower left is (0,0)

            armColor = ((Texture2D)charSkin).GetPixel(1, 30 - 15);

            handColor = ((Texture2D)charSkin).GetPixel(1, 30 - 16);

        }
        else
        {
            shoulderColor = ((Texture2D)charSkin).GetPixel(5, 30 - 14);

            armColor = ((Texture2D)charSkin).GetPixel(5, 30 - 15);

            handColor = ((Texture2D)charSkin).GetPixel(5, 30 - 16);

        }

        for (int i = 0; i < transform.childCount; i++)
        {

            if (transform.GetChild(i).name.Contains("Pixel"))
            {


                if (i == 0)  //color the first pixel the shoulder color
                {
                    transform.GetChild(i).GetComponent<SpriteRenderer>().color = shoulderColor;
                }
                else
                {
                    transform.GetChild(i).GetComponent<SpriteRenderer>().color = armColor;
                }

            }

            if (transform.GetChild(i).name.Contains("Hand"))
            {

                theHand = transform.GetChild(i);

                theHand.GetComponent<SpriteRenderer>().color = handColor;

            }

        }

    }

    // Start is called before the first frame update
    void Start()
    {

        firstChild = transform.GetChild(0);

        unitWidth = firstChild.transform.localScale.x * firstChild.GetComponent<SpriteRenderer>().size.x;

        //Debug.Log(unitWidth);

        childcount = transform.childCount;

        gunhold = transform.GetChild((int)childcount - 1);

        theparent = transform;

        thechild = transform.GetChild(0);


        //Vector3 gunpointpos = gunpoint.position;


    }

    // Update is called once per frame
    void Update()
    {


        gunpointposition = gunpoint.position;

        //theHand.position = gunpointposition;
        //Debug.Log(gunhold.position);

        //unitWidth = offset;

        //SetPosition(transform, transform.GetChild(0));
        //if (!done)
        //{
        //    Debug.Log(transform.name + " " + transform.GetChild(0).name);
        //}
        //SetPosition(transform.GetChild(0), transform.GetChild(1));
        //if (!done)
        //{
        //    Debug.Log(transform.GetChild(0) + " " + transform.GetChild(1).name);
        //}
        //SetPosition(transform.GetChild(1), transform.GetChild(2));
        //if (!done)
        //{
        //    Debug.Log(transform.GetChild(1) + " " + transform.GetChild(2).name);
        //}
        //SetPosition(transform.GetChild(2), transform.GetChild(3));
        //if (!done)
        //{
        //    Debug.Log(transform.GetChild(2) + " " + transform.GetChild(3).name);
        //}


        for (int i = 0; i < transform.childCount - 2; i++)      //-2 ALSO does the same thing v
        {

            if (!transform.GetChild(i).name.Contains("Hand"))
            {   //do not do the operation for the hand.

                if (i == 0)
                {

                    SetPosition(transform, transform.GetChild(i), true);

                }
                else
                {

                    SetPosition(transform.GetChild(i - 1), transform.GetChild(i), false);

                }
            }


        }

    }

    //void OnDrawGizmos()
    //{
    //    
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, unitWidth);
    //
    //    Gizmos.DrawWireSphere(transform.position, unitWidth * childcount);
    //}

    void SetPosition(Transform parent, Transform child, bool isfirst)
    {


        float diametermultiplier = 0;

        if (isfirst)
        {

            diametermultiplier = (unitWidth - offset) / 10f;
        }
        else
        {

            diametermultiplier = unitWidth - offset;
        }

        //gunpointposition.z = 0;


        Vector3 difference = new Vector3(gunpointposition.x - parent.position.x, gunpointposition.y - parent.position.y, 0);

        Vector3 newPos = new Vector3(parent.position.x + difference.x * diametermultiplier / difference.magnitude, parent.position.y + difference.y * diametermultiplier / difference.magnitude, transform.position.z);

        //Debug.DrawLine(new Vector3(parent.position.x + difference.x, parent.position.y + difference.y, 0), gunpointposition, Color.red);

        if (difference.magnitude < 0.01f || parent.gameObject.activeSelf == false)
        {
        
            child.gameObject.SetActive(false);
        
            if(parent.gameObject.activeSelf == true){   //if this is the last active pixel
        
                parent.gameObject.SetActive(false);
                theHand.position = parent.position;
            }
        }
        else
        {
            child.gameObject.SetActive(true);
            child.position = newPos;
        }



    }

}