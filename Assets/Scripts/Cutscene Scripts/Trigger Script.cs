using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;
    public GameObject phoneMenu;
    public Animator playerAnimator; // Reference to the Animator component
    public Animator phoneAnimator;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            RingPhone();



            //Debug.Log("Hey");
            //
            //other.GetComponent<PlayerMovement>().enabled = false;
            //
            //other.GetComponent<PlayerRotation>().enabled = false;
            //
            //playerAnimator.SetTrigger("takingOutPhone");
            //
            //initiateTexting();

        }
    }

    void TakeOutPhone()
    {

        playerAnimator.SetTrigger("takingOutPhone");

        phoneAnimator.SetTrigger("isTakenOut");

        phoneAnimator.SetBool("isOut", true);

    }


    void RingPhone()
    {

        playerAnimator.SetBool("isInCutscene", true);



        playerAnimator.SetFloat("DirectionX", 0f);

        playerAnimator.SetFloat("DirectionY", 0f);

        playerAnimator.SetFloat("Speed", 0f);

        playerAnimator.SetFloat("Horizontal", 0f);

        playerAnimator.SetFloat("Vertical", 0f);

        playerAnimator.SetBool("isLooking", false);

        playerAnimator.Play("Map_Idle");



        phoneAnimator.SetBool("isRinging", true);

        player.GetComponent<PlayerMovement>().enabled = false;

        player.GetComponent<PlayerRotation>().enabled = false;



        StartCoroutine(StopRinging(4f)); //2.5 seconds 


    }

    private System.Collections.IEnumerator StopRinging(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Set the "isRinging" parameter to true after the delay

        Debug.Log("stopping ringing");

        phoneAnimator.SetBool("isRinging", false);

        TakeOutPhone();

    }

    void InitiateTexting()
    {

        RectTransform rectTransform = phoneMenu.GetComponent<RectTransform>();

        Vector3 newPosition = rectTransform.anchoredPosition;
        newPosition.y += 100f; // Adjust the value based on your needs
        rectTransform.anchoredPosition = newPosition;
    }


    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

}
