using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneMessageManagerScript : MonoBehaviour
{



    public GameObject gameobjMsg; // Template GameObject
    public int maxMessages = 6;
    private List<GameObject> shownMessages = new List<GameObject>();

    void Start()
    {
        // Ensure the template is inactive but ready to use
        gameobjMsg.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            AddMessage("Hey, this is a new message", true);
        }

    }



    void AddMessage(string msgTxt, bool isIncoming)
    {
        // Move existing messages up
        foreach (GameObject message in shownMessages)
        {
            RectTransform rectTransform = message.GetComponent<RectTransform>();
            rectTransform.localPosition += new Vector3(0, 17.8f, 0); // Adjust Y by 20
        }

        // Remove the oldest message if exceeding maxMessages
        if (shownMessages.Count >= maxMessages)
        {
            Destroy(shownMessages[0]);
            shownMessages.RemoveAt(0);
        }

        // Instantiate a new message at the position of gameobjMsg
        GameObject newMessage = Instantiate(gameobjMsg, gameobjMsg.transform.parent);
        newMessage.SetActive(true); // Activate the new message


        newMessage.transform.localPosition = gameobjMsg.transform.localPosition;

        // Set the message text and color
        newMessage.GetComponentInChildren<TextMeshProUGUI>().text = msgTxt;
        Image messageImage = newMessage.GetComponent<Image>();
        messageImage.color = isIncoming ? Color.white : Color.red;

        Animation animation = newMessage.GetComponent<Animation>();

        if (animation != null)
        {
            animation.Play("Phone_MsgPop");
        }


        // Add the new message to the list
        shownMessages.Add(newMessage);
    }


}

