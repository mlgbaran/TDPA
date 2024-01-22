using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField] Camera cam;

    public Boolean isVisible = true;


    // Update is called once per frame
    void Update()
    {



        transform.position = new Vector3(cam.ScreenToWorldPoint(Input.mousePosition).x, cam.ScreenToWorldPoint(Input.mousePosition).y, 2);
        //  transform.position.z = 2;


    }
}
