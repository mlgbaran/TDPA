using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class IsoObjectSorterScript : MonoBehaviour
{
    public Vector2 pointA = new Vector2(-0.03f, -0.08f);
    public Vector2 pointB = new Vector2(0.04f, -0.05f);

    public Vector2 GetObjectPosition()
    {
        return transform.position;
    }

    public Vector2 GetMidpoint()
    {
        return (pointA + pointB) / 2;
    }

}