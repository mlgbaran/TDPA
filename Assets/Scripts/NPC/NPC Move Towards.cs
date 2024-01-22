using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMoveTowards : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform target;

    public bool moveTowardsTheObject;
    public float moveSpeed = 0.77f;

    public Rigidbody2D rb;

    public Animator animator;

    public float movementX;

    public float movementY;

    public float directionx;

    public float directiony;

    Vector2 movement;

    // Update is called once per frame
    void Update()
    {

        //movementX = movement.x;
        //movementY = movement.y;
        //
        //movement.x = Input.GetAxisRaw("Horizontal");
        //movement.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("Horizontal", directionx);
        animator.SetFloat("Vertical", directiony);



    }

    void FixedUpdate()
    {

        if (moveTowardsTheObject && target != null)
        {

            Vector2 direction = target.position - transform.position;

            if (direction.magnitude > 0.01f)
            {
                //Debug.Log(direction.magnitude);
                // Calculate the direction to the target.

                // Normalize the direction vector to maintain constant speed.
                direction.Normalize();

                directionx = MyClamp(direction.x);
                directiony = MyClamp(direction.y);

                animator.SetFloat("Speed", direction.sqrMagnitude);

                // Move the character toward the target.
                transform.Translate(direction * moveSpeed * Time.deltaTime);


            }
            else
            {

                animator.SetFloat("Speed", 0f);
            }



        }
        else
        {

            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        }




    }

    public void HalveMoveSpeed()
    {
        Debug.Log("Movement Speed Reduced");
        moveSpeed = moveSpeed / 2;
    }
    public void NormalMoveSpeed()
    {
        Debug.Log("Movement Speed Set Back To Normal");
        moveSpeed = moveSpeed * 2;
    }

    private float MyClamp(float input)
    {

        if (input > 0.5f)
        {

            return 1;
        }
        else if (input < -0.5f)
        {

            return -1;

        }
        else
        {
            return 0;
        }


    }




}
