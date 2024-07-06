using System;
using System.Collections;
using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    public GameObject player;
    public GameObject phoneMenu;
    public Animator playerAnimator; // Reference to the Animator component
    public Animator phoneAnimator;

    private bool isTexting = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(PlayAnimationSequence());
        }
    }

    private IEnumerator PlayAnimationSequence()
    {

        StopAllPlayerAnimations();

        playerAnimator.Play("Map Idle");

        // Step 1: Play Map_Look_Down and Phone_Ring


        //playerAnimator.Play("Map_Look_Down");
        phoneAnimator.SetBool("isRinging", true);
        //playerAnimator.Play("Map_Idle"); // Reset to a neutral idle state



        yield return new WaitForSeconds(2.0f);

        // Step 2: Play Text_TakeOut and Phone_TakeOut
        playerAnimator.SetTrigger("takingOutPhone");


        yield return new WaitForSeconds(0.7f);
        phoneAnimator.SetBool("isRinging", false);
        phoneAnimator.SetTrigger("isTakenOut");
        phoneAnimator.SetBool("isOut", true);
        //yield return new WaitUntil(() => AnimationIsPlaying(playerAnimator, "Text Take Out") == false);

        // Step 3: Transition to Text_Text and Phone_OutIdle
        //playerAnimator.Play("Text Text");
        //phoneAnimator.Play("Phone_OutIdle");

        isTexting = true;
    }

    private void Update()
    {
        if (isTexting && Input.GetKeyDown(KeyCode.F))
        {
            StartCoroutine(EndTexting());
        }
    }

    private IEnumerator EndTexting()
    {
        isTexting = false;

        // Step 4: Play Text_PutDown
        playerAnimator.SetTrigger("puttingDownPhone");

        playerAnimator.Play("Text Put Down");

        yield return new WaitForSeconds(1f);



        // Optionally, reset or transition to another state
        phoneAnimator.SetBool("isOut", false);
        phoneAnimator.SetTrigger("isPutDown");
        playerAnimator.SetBool("isInCutscene", false);
        player.GetComponent<PlayerMovement>().enabled = true;
        player.GetComponent<PlayerRotation>().enabled = true;
    }

    private bool AnimationIsPlaying(Animator animator, string stateName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1;
    }

    private void StopAllPlayerAnimations()
    {
        player.GetComponent<PlayerMovement>().enabled = false;
        player.GetComponent<PlayerRotation>().enabled = false;
        // Reset all parameters to ensure no other animations are playing
        playerAnimator.SetBool("isInCutscene", true);
        playerAnimator.SetFloat("DirectionX", 0f);
        playerAnimator.SetFloat("DirectionY", 0f);
        playerAnimator.SetFloat("Speed", 0f);
        playerAnimator.SetFloat("Horizontal", 0f);
        playerAnimator.SetFloat("Vertical", 0f);
        playerAnimator.SetBool("IsLooking", false);         //IS LOOKING IS WITH A BIG I, DIFFERENT THAN THE OTHERS!!!!!!!!!!!!!!!
        playerAnimator.SetBool("isEquippedPistol", false);
        playerAnimator.SetBool("isTalking", false);

    }

    void Start()
    {
    }

    void InitiateTexting()
    {
        RectTransform rectTransform = phoneMenu.GetComponent<RectTransform>();
        Vector3 newPosition = rectTransform.anchoredPosition;
        newPosition.y += 100f; // Adjust the value based on your needs
        rectTransform.anchoredPosition = newPosition;
    }
}
