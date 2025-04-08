using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public CharacterController body;
    public float speed = 7f;
    public float jumpForce = 8f;
    public float maxSpeed = 12f;
    public float gravity = -20f;
    public float friction = 0.1f;
    float lastGroundedTime;
    int consecutiveJumps;

    float horizontal;
    float vertical;

    bool isGrounded;
    float playerHeight = 5.0f;
    public LayerMask groundMask;

    Vector3 move;
    Vector3 velocity;


    void Update()
    {
        myInput();
        groundCheck();
        movement();
    }

    void groundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight / 2 + 0.1f, groundMask);
        
        if(isGrounded)
        {
            lastGroundedTime = Time.time;
        }
    }

    void myInput()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
    }

    void movement() { 
        move = transform.forward * vertical + transform.right * horizontal;
        move.Normalize();
        if (isGrounded)
        {
           

            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = jumpForce;
                float timeSinceGrounded = Time.time - lastGroundedTime;

                // Apply bunny hop boost if jumped quickly after landing
                if (timeSinceGrounded < 0.1f)
                {
                    consecutiveJumps++;

                    // Calculate speed multiplier that increases with each consecutive hop
                    float speedMultiplier = 5.0f + (consecutiveJumps * 8.0f);

                    // Cap the multiplier if needed
                    //speedMultiplier = Mathf.Min(speedMultiplier, 25.0f);

                    // Preserve existing momentum and add more based on input direction
                    velocity.x += move.x * speed * speedMultiplier;
                    velocity.z += move.z * speed * speedMultiplier;
                }
                else
                {
                    consecutiveJumps = 0;
                }
                
            }
            if (isGrounded && Time.time - lastGroundedTime > 0.2f)
            {
                consecutiveJumps = 0;
            }

            if (move.magnitude < 1.0f)
            {
                velocity.x = 0f;
                velocity.z = 0f;
            }
        }
        velocity.x += move.x * speed;
        velocity.z += move.z * speed;

        // Limit horizontal speed
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        if (horizontalVelocity.magnitude > maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
            velocity.x = horizontalVelocity.x;
            velocity.z = horizontalVelocity.z;
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Move the character
        body.Move(velocity * Time.deltaTime);

    }
   
}