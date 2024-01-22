using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCLookDirection : MonoBehaviour
{   

    public GameObject LookObject;

    public bool lookAtObject;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
 


            if (LookObject != null && lookAtObject)
            {
            
                transform.position = LookObject.transform.position;
    
            }


        
    }
}
