using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class SetGunPoint : MonoBehaviour
{

    public Camera cam;
    public Transform L_shoulder;

    public Transform R_shoulder;

    public Transform targetPosition;

    private float childcount;

    private float unitWidth;

    private Transform gunpoint;

    public float diameter = 5.81f;

    private Transform root;

    private bool isPlayer;

    // Start is called before the first frame update
    void Start()
    {

        childcount = L_shoulder.childCount;

        unitWidth = unitWidth = L_shoulder.GetChild(0).transform.localScale.x * L_shoulder.GetChild(0).GetComponent<SpriteRenderer>().size.x;

        gunpoint = transform.GetChild(0);

        root = transform.root;

        isPlayer = root.gameObject.tag == "Player";

    }

    void OnDrawGizmos()
    {

        Gizmos.color = Color.yellow;
        //Gizmos.DrawWireSphere(transform.position, unitWidth);

        Gizmos.DrawWireSphere(transform.position, unitWidth * (childcount - diameter));

    }

    void OnValidate()
    {
        //if (diameter >= childcount)
        //{
        //    diameter = (float)childcount - 0.01f;
        //}
    }

    // Update is called once per frame
    void Update()
    {
        //setting the center position of the circle
        if (isPlayer)
        {
            SetGunPointPos(cam.ScreenToWorldPoint(Input.mousePosition));
        }
        else
        {
            SetGunPointPos(targetPosition.position);
        }


    }

    void SetGunPointPos(Vector3 targetpos)
    {

        Vector3 newpos = new Vector3((L_shoulder.position.x + R_shoulder.position.x) / 2, (L_shoulder.position.y + R_shoulder.position.y) / 2, 0);

        transform.position = newpos;

        targetpos.z = 0;

        Vector3 difference = new Vector3(targetpos.x - transform.position.x, targetpos.y - transform.position.y, 0);

        Vector3 newgunpointpos = new Vector3(transform.position.x + difference.x * unitWidth * (childcount - diameter) / difference.magnitude, transform.position.y + difference.y * unitWidth * (childcount - diameter) / difference.magnitude, 0);

        gunpoint.position = newgunpointpos;


    }
}
