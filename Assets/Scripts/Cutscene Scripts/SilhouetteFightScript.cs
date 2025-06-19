using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilhouetteFightScript : MonoBehaviour


{

    public GameObject playerGO;

    public Animator animator;

    private bool isStarted = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (playerGO.transform.position.x > 6.3549f && !isStarted)
        {
            StartAnimation();
        }

    }

    private void StartAnimation()
    {

        isStarted = true;

        animator.SetBool("Started", true);

        playerGO.SetActive(false);



    }
}
