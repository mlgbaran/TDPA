using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 0.77f;

    public bool sideView = false;

    public Rigidbody2D rb;

    public Animator animator;

    public float movementX;

    public float movementY;

    public float calculatedSpeed;

    Vector2 movement;

    // Update is called once per frame
    void Update()
    {
        if (sideView == false)
        {
            movementX = movement.x;
            movementY = movement.y;

            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            animator.SetFloat("Horizontal", movement.x);
            animator.SetFloat("Vertical", movement.y);

            int horizontalInt = 0;
            int verticalInt = 0;
            if (movement.x < -0.1f) horizontalInt = -1;
            else if (movement.x > 0.1f) horizontalInt = 1;

            if (movement.y < -0.1f) verticalInt = -1;
            else if (movement.y > 0.1f) verticalInt = 1;

            animator.SetInteger("HorizontalInt", horizontalInt);
            animator.SetInteger("VerticalInt", verticalInt);

            calculatedSpeed = movement.sqrMagnitude;

            animator.SetFloat("Speed", calculatedSpeed);

        }
        else
        {
            movementX = movement.x;
            movementY = 0;
            movement.x = Input.GetAxisRaw("Horizontal");
            animator.SetFloat("Horizontal", movement.x);
            calculatedSpeed = movement.sqrMagnitude;
            animator.SetFloat("Speed", calculatedSpeed);
        }


    }

    public void StopMoving()    //stopping the movement and resetting the animator variables
    {
        movementX = 0;

        movementY = 0;

        calculatedSpeed = 0;

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", calculatedSpeed);
    }

    void FixedUpdate()
    {


        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);



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

}
