using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class PlayerCombat : MonoBehaviour
{


    public Animator animator;





    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {

            Attack();
        }
    }

    void Attack()
    {


        animator.SetTrigger("Attack");


    }
}
