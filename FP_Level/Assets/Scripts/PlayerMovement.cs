using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float playerSpeed = 1f;
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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        m_ObjectCollider = playerModel.GetComponent<Collider>();
    }

    private void Update()
    {
        MyInput();
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

    private void MyInput()
    {
        // WASD Movement
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(jumpKey) & grounded)
        {
            Jump();
        }
        if (Input.GetKey(sprintKey) & grounded)
        {
            playerSpeed = baseSpeed * 1.5f;
        } else
        {
            playerSpeed = baseSpeed;
        }
        if (Input.GetKeyDown(dashKey))
        {
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

    // Check grounded & Respawn 
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground")) 
        {
            groundCollisions++;
            grounded = true;
        } else if (other.gameObject.CompareTag("Hazard")) 
        {
            this.transform.position = respawn.transform.position;
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
