using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{

    // Variables
    [Header("Movement")]
    public float playerSpeed = 1f;
    public float baseSpeed = 1f;

    public float groundDrag;
    public float airMultiplier;
    public float jumpForce;
    public float dashForce;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey;
    public KeyCode sprintKey;

    [Header("ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;
    private int groundCollisions;
    private bool nullDrag;

    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;
    Collider m_ObjectCollider;

    public GameObject playerModel;
    public GameObject respawn;
    // private movement vars
    public bool dashUnlocked = false;
    bool canDash = false;
    bool doubleJump;
    int jumps = 1;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        m_ObjectCollider = playerModel.GetComponent<Collider>();
    }

    private void Update()
    {
        PlayerInputs();
        SpeedControl();

        // Drag
        if(grounded && !nullDrag)
        {
            rb.drag = groundDrag;
        }
        else {
            rb.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void PlayerInputs()
    {
        // WASD Movement
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Jumping
        if (Input.GetKeyDown(jumpKey) && grounded)
        {
            if (grounded)
            {
                Jump();
            }
            else if (jumps > 0)
            {
                jumps--;
                Jump();
            }
        } 

        // Sprinting
        if (grounded)
        {
            if (Input.GetKey(sprintKey))
            {
                playerSpeed = baseSpeed * 1.5f;
            }
            else
            {
                playerSpeed = baseSpeed;
            }

            if (Input.GetKeyDown(dashKey) && canDash)
            {
                Dash();
            }
        } else if (Input.GetKeyDown(dashKey) && canDash)
        {
            canDash = false;
            Dash();
        }
    }

    private void MovePlayer()
    {
        // move direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // grounded
        if(grounded)
        {
            rb.AddForce(moveDirection.normalized * playerSpeed * 40f, ForceMode.Force);
        } else {
            rb.AddForce(moveDirection.normalized * playerSpeed * airMultiplier, ForceMode.Force);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb. velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > playerSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * playerSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    // Check grounded
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            groundCollisions++;
            grounded = true;

            if (doubleJump)
            {
                jumps = 2;
            }
            else
            {
                jumps = 1;
            }

            if (dashUnlocked)
            {
                canDash = true;
            }
        }
        else if (other.gameObject.CompareTag("Hazard"))
        {
            this.transform.position = respawn.transform.position;
        }
        else if (other.gameObject.CompareTag("Checkpoint"))
        {
            if (respawn != null)
            {
                if (respawn.GetComponent<Checkpoint>())
                {
                    respawn.GetComponent<Checkpoint>().DeactivateBeacon();
                }
            }
            respawn = other.gameObject;
            respawn.GetComponent<Checkpoint>().ActivateBeacon();
        }
        else if (other.gameObject.CompareTag("DashPellet"))
        {
            dashUnlocked = true;
            canDash = true;
            Destroy(other.gameObject);
        }
    }

    // Check grounded
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ground")) 
        {
            groundCollisions--;

            if (groundCollisions < 1)
            {
                grounded = false;
            }
         }
    } 

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce * 10, ForceMode.Impulse);
    }

    private void Dash()
    {
        grounded = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce * 2, ForceMode.Impulse);
        rb.AddForce(moveDirection.normalized * dashForce * 1000f, ForceMode.Impulse);
    }
}
