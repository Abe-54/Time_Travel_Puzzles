using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public bool isFacingRight = true;
    public float speed = 10.0f;

    public float jumpForce = 10.0f;
    public bool isGrounded = false;
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundMask;

    public CinemachineVirtualCamera presentVCam;
    public CinemachineVirtualCamera pastVCam;
    public bool isPast = false;

    private float horizontalInput;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);

        Flip();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            isPast = !isPast;
            SwapCamera();
        }
    }

    void FixedUpdate()
    {
        Move(horizontalInput);

    }

    public void Move(float moveInput)
    {
        if (Mathf.Abs(moveInput) > 0.01f) // Player is moving
        {
            // Directly set the velocity for immediate response
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        }
        else // Player has released movement controls
        {
            // Immediately stop the player by setting horizontal velocity to zero
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }


    void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }

    void Flip()
    {
        if (isFacingRight && horizontalInput < 0 || !isFacingRight && horizontalInput > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    void SwapCamera()
    {
        if (isPast)
        {
            presentVCam.Priority = 0;
            pastVCam.Priority = 1;
        }
        else
        {
            presentVCam.Priority = 1;
            pastVCam.Priority = 0;
        }
    }
}
