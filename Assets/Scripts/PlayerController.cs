using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float acceleration = 10.0f;
    [Range(0f, 1f)]
    public float groundDecay = 5.0f;
    public float speed = 10.0f;
    public bool canMove = true;

    [Space(10)]
    [Header("Jumping")]
    public float jumpForce = 10.0f;

    [Header("Ground Check")]
    public bool isGrounded = false;
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundMask;

    [Space(10)]
    [Header("Inventory")]
    private GameObject itemToPickUp;
    public GameObject carryingItem;
    public Transform itemHoldPosition;

    [Header("Debugging")]
    public bool stopInAir = false;

    private float xInput;
    private Rigidbody2D body;
    private UIManager uiManager;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        uiManager = FindObjectOfType<UIManager>();
        gameManager = FindObjectOfType<GameManager>();

        gameManager.currentTimePeriod = GameManager.TimePeriod.Present;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove)
        {
            return;
        }

        CheckInput();
        HandleJump();

        if (Input.GetKeyDown(KeyCode.E) && itemToPickUp != null && carryingItem == null)
        {
            pickupItem(itemToPickUp); // Pick up an item from the ground
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (itemToPickUp != null && itemToPickUp.CompareTag("Tree"))
            {
                InteractWithTree(itemToPickUp);
            }
            else if (carryingItem != null)
            {
                DropItem();
            }
            else if (itemToPickUp != null)
            {
                takeItem(itemToPickUp);
            }
        }
    }

    void FixedUpdate()
    {
        if (!canMove)
        {
            return;
        }

        CheckGround();
        Move();
        ApplyFriction();
    }

    void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            gameManager.SwapTimePeriod();
        }
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);
    }

    void InteractWithTree(GameObject tree)
    {
        uiManager.HideInteractText();
        gameManager.knockedTree.SetActive(true);
        tree.SetActive(false);
    }

    public void Move()
    {
        if (Mathf.Abs(xInput) > 0f) // Player is moving
        {

            float increment = xInput * acceleration;
            float newSpeed = Mathf.Clamp(body.velocity.x + increment, -speed, speed);

            // Directly set the velocity for immediate response
            body.velocity = new Vector2(newSpeed, body.velocity.y);

            float direction = Mathf.Sign(xInput);
            transform.localScale = new Vector3(direction, 1, 1);
        }
        else if (stopInAir && !isGrounded) // Player is not moving and in the air
        {
            body.velocity = new Vector2(0, body.velocity.y);
        }
    }


    void HandleJump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            body.velocity = new Vector2(body.velocity.x, jumpForce);
        }
    }

    void ApplyFriction()
    {
        if (isGrounded && xInput == 0 && body.velocity.y <= 0f)
        {
            body.velocity *= groundDecay;
        }
    }

    public void takeItem(GameObject itemToPickUp)
    {
        if (itemToPickUp != null)
        {
            // Instantiate or directly assign the item to the player
            carryingItem = Instantiate(itemToPickUp, itemHoldPosition.position, Quaternion.identity, itemHoldPosition);
            carryingItem.GetComponent<Rigidbody2D>().isKinematic = true;
            carryingItem.GetComponent<Collider2D>().enabled = false;

            uiManager.UpdateInventory(carryingItem); // Update UI to show the picked-up item
        }
    }

    public void pickupItem(GameObject itemFromGround)
    {
        if (itemFromGround != null && carryingItem == null) // Ensure the player isn't already carrying an item
        {
            carryingItem = itemFromGround; // Assign the existing item to be the carried item
            carryingItem.transform.position = itemHoldPosition.position; // Move it to the hold position
            carryingItem.transform.SetParent(itemHoldPosition); // Parent it to the hold position
            carryingItem.GetComponent<Rigidbody2D>().isKinematic = true; // Make it kinematic to prevent physics interactions while carried
            carryingItem.GetComponent<Collider2D>().enabled = false; // Optionally disable the collider

            uiManager.UpdateInventory(carryingItem); // Update the UI to show the picked-up item
        }
    }

    public void DropItem()
    {
        // Drop the item
        if (carryingItem != null)
        {
            // Logic to drop the item in the world
            carryingItem.transform.SetParent(null); // Detach the item from the player
            carryingItem.GetComponent<Rigidbody2D>().isKinematic = false;
            carryingItem.GetComponent<Collider2D>().enabled = true;
            // carryingItem.transform.position = transform.position + transform.right; // Adjust as necessary

            uiManager.ClearInventory(); // Update UI to remove the item
            carryingItem = null; // Clear the reference
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Interactable") && carryingItem == null)
        {
            uiManager.ShowInteractText();

            if (other.gameObject.GetComponent<BagController>() != null)
            {
                itemToPickUp = other.gameObject.GetComponent<BagController>().itemToDispense;
            }
        }

        if (other.gameObject.GetComponent<Seed>() != null && carryingItem == null)
        {
            itemToPickUp = other.gameObject;
        }

        if (other.gameObject.CompareTag("Tree"))
        {
            uiManager.ShowInteractText();
            itemToPickUp = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            uiManager.HideInteractText();

            if (other.gameObject.GetComponent<BagController>() != null)
            {
                itemToPickUp = null;
            }
        }

        if (other.gameObject == itemToPickUp)
        {
            itemToPickUp = null;
            uiManager.HideInteractText();
        }
    }
}
