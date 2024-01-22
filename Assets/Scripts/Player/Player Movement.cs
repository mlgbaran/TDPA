using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 0.77f;

    public Rigidbody2D rb;

    public Animator animator;

    public float movementX;

    public float movementY;

    Vector2 movement;

    // Update is called once per frame
    void Update()
    {

        movementX = movement.x;
        movementY = movement.y;

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);

        animator.SetFloat("Speed", movement.sqrMagnitude);

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
