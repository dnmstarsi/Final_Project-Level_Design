using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float playerSpeed = 1f;

    public float groundDrag;
    public float airMultiplier;
    public float jumpForce;
    public float dashForce;
    public float quantumTimer;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode dashKey;
    public KeyCode sprintKey;

    [Header("ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;
    private bool nullDrag;

    private bool collision;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    Collider m_ObjectCollider;
    public GameObject playerModel;

    public GameObject respawn;

    public TextMeshProUGUI PhasestepTimer;

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
            playerSpeed = 20;
        } else
        {
            playerSpeed = 10;
        }
        if (Input.GetKeyDown(dashKey))
        {
            Dash();
        }
        /*
        // Card Abilites
        if(Input.GetKeyDown(jumpKey) && !script.onDeckpoint)
        {
            if (script.selection.Count > 0)
            {
                if (script.selection[0].tag == "Jump")
                {
                    if (grounded)
                    {
                        Destroy(script.selection[0]);
                        script.selection.RemoveAt(0);
                        Jump();
                    }
                } else if (script.selection[0].tag == "Dash")
                {
                    Destroy(script.selection[0]);
                    script.selection.RemoveAt(0);
                    Dash();
                } else if (script.selection[0].tag == "Quantum")
                {
                    Destroy(script.selection[0]);
                    script.selection.RemoveAt(0);
                    Quantum();
                } 
            }

            script.SetHUD();
        } */
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
        collision = false;

        if (other.gameObject.CompareTag("Ground")) 
        {
            grounded = false;
        }
    } 
    private void OnTriggerEnter(Collider other)
    {
        collision =true;

        if (other.gameObject.CompareTag("Ground")) 
        {
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
        grounded = false;
    }

    private void Dash()
    {
        grounded = false;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce * 2, ForceMode.Impulse);
        rb.AddForce(moveDirection.normalized * dashForce * 1000f, ForceMode.Impulse);
    }

    private void Quantum()
    {
        m_ObjectCollider.isTrigger = true;
        nullDrag = true;

        PhasestepTimer.text = "Quantum Tunneling Activated";

        StartCoroutine(QuantumTermination());
    }

    IEnumerator QuantumTermination()
    {
        yield return new WaitForSeconds(quantumTimer);

        m_ObjectCollider.isTrigger = false;
        nullDrag = false;

        PhasestepTimer.text = "";

        if (collision) 
        {
            this.transform.position = respawn.transform.position;
        }
    }
}
