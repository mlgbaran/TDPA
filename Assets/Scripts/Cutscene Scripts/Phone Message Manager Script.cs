using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PhoneMessageManagerScript : MonoBehaviour
{

    public GameObject gameobjMsg;

    public int maxMessages = 6;

    public List<GameObject> shownMessages = new List<GameObject>();

    void AddMessage(string msgTxt, bool isIncoming)
    {

        if (shownMessages.Count == maxMessages)
        {

            shownMessages.RemoveAt(0);

        }

        GameObject newMessage = Instantiate(gameobjMsg);

        if (isIncoming)
        {

            

        }
        else
        {

            newMessage.GetComponent<Image>().tintColor = Color.red;

        }

        shownMessages.Add(newMessage);



    }




    // Start is called before the first frame update
    void Start()
    {



    }

    // Update is called once per frame
    void Update()
    {

    }
}
