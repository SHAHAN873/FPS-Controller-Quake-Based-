using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMove : MonoBehaviour
{
    [Header ("Movement Settings")]
    [SerializeField] CharacterController body;
    [SerializeField] float speed = 15f;
    [SerializeField] float jumpForce = 8f;
    [SerializeField] float maxSpeed = 20f;
    [SerializeField] float gravity = -20f;
    [SerializeField] float bHopMaxSpeed = 80f;
    [SerializeField] LayerMask groundMask;

    //Player state variables
    bool isGrounded;
    float playerHeight = 6.0f;
    float lastGroundedTime;
    int consecutiveJumps;
    float defaultMaxSpeed;

    Vector3 move;
    Vector3 velocity;
    
    void Start()
    {
        //Initialize the character Default max speed to normal speed
        defaultMaxSpeed = maxSpeed;
    }

    void Update()
    {
        //grounded and movement checks
        groundCheck();
        movement();
    }

    //Check if the player is grounded
    void groundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight/3 + 0.1f, groundMask);
        if (isGrounded)
        {
            // Reset the jump count when grounded
            lastGroundedTime = Time.time;
        }
    }

    

    void movement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        move = transform.forward * vertical + transform.right * horizontal;
        move.Normalize();

        if (isGrounded)
        {
            // Reset the jump count when grounded
            if ((Time.time - lastGroundedTime) < 0.3f)
            {
                consecutiveJumps++;
            }
            else if ((Time.time - lastGroundedTime) > 0.2f)
            {
                consecutiveJumps = 0;
            }

            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = jumpForce;
                isGrounded = true;
            }

            //Slowing down character when input is not given
            if (move.magnitude < 0.1f)
            {
                velocity.x = Mathf.Lerp(velocity.x,0f,0.1f);
                velocity.z = Mathf.Lerp(velocity.z, 0f, 0.1f);
            }
        }
        // Apply movement
        velocity.x += move.x * speed;
        velocity.z += move.z * speed;

        //Bunny hopping and in-air movement 
        if (!isGrounded && consecutiveJumps > 2)
        {
            velocity.x += move.x * consecutiveJumps;
            velocity.z += move.z * consecutiveJumps;
            defaultMaxSpeed = Mathf.Lerp(defaultMaxSpeed, bHopMaxSpeed, Time.deltaTime * 2f);
            isGrounded = false;
        }
        else
        {
            // Reset the max speed to normal when grounded
            defaultMaxSpeed = Mathf.Lerp(defaultMaxSpeed, maxSpeed, Time.deltaTime * 2f);
        }

        // Limit the maximum speed
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        if (horizontalVelocity.magnitude > defaultMaxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * defaultMaxSpeed;
            velocity.x = horizontalVelocity.x;
            velocity.z = horizontalVelocity.z;
        }

       

        // Applying gravity
        velocity.y += gravity * Time.deltaTime;

        // Move the character
        body.Move(velocity * Time.deltaTime);

    }
    void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.normal.textColor = Color.black;
        //Speed display
        GUI.Label(new Rect(10, 10, 200, 20), "Speed: " + new Vector3(velocity.x, 0, velocity.z).magnitude.ToString("F1"),style);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Check if the player is colliding with a wall
        if (hit.gameObject.CompareTag("Trigger"))
        {
            // Reset the jump count when colliding with a wall
            if(Input.GetButtonDown("Jump"))
            {
                velocity.y = jumpForce;
                isGrounded = true;
            }
            Debug.Log("Collided with wall");
        }
    }

}

