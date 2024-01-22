using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class GunHoldScript : MonoBehaviour
{

    public Transform gunpoint;
    // Start is called before the first frame update
    void Start()
    {


        //Debug.Log(unitWidth);

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(gunpoint.position.x, gunpoint.position.y, 0);

    }
}
