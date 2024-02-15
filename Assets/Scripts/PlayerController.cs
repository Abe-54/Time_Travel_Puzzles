using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    public bool isFacingRight = true;
    public float speed = 10.0f;

    // public Transform pastSpawnPoint;
    // public Transform presentSpawnPoint;

    public float jumpForce = 10.0f;
    public bool isGrounded = false;
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundMask;

    public CinemachineVirtualCamera presentVCam;
    public CinemachineVirtualCamera pastVCam;
    public bool isPast = false;

    public GameObject item;
    public GameObject pastPlayer;
    public GameObject presentPlayer;

    private float horizontalInput;
    private Rigidbody2D rb;
    private UIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        uiManager = FindObjectOfType<UIManager>();

        presentVCam.Priority = 1;
        pastVCam.Priority = 0;

        pastPlayer.SetActive(false);
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
            StartCoroutine(SwapTimePeriod());
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (item != null)
            {
                pickUpItem(item);
                Destroy(item);
            }
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

    IEnumerator SwapTimePeriod()
    {
        pastPlayer.SetActive(isPast);
        presentPlayer.SetActive(!isPast);
        SwapCamera();


        yield return new WaitForSeconds(0.1f);

    }

    public void pickUpItem(GameObject itemToPickUp)
    {
        uiManager.UpdateInventory(itemToPickUp);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Pick-Up"))
        {
            item = other.gameObject;
        }
    }
}
