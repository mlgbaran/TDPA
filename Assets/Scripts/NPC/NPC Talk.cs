using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTalk : MonoBehaviour
{


    public Animator animator;

    public float animationCooldown;
    public bool isTalking;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlayTalkingAnimation());
    }

    IEnumerator PlayTalkingAnimation()
    {
        while (isTalking)
        {
            // Roll a dice between 1 and 10
            int randomTalkNumber = Random.Range(1, 11);

            // Set the "talkDice" parameter in the animator
            animator.SetFloat("talkDice", randomTalkNumber);

            // Wait for the current animation to finish
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

            // Wait for a short delay before rolling the dice for the next animation
            yield return new WaitForSeconds(animationCooldown); // Adjust this delay as needed
        }
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("isTalking", isTalking);
    }
}
