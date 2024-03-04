using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float acceleration = 10.0f;
    [Range(0f, 1f)]
    public float groundDecay = 5.0f;
    public float speed = 10.0f;
    public float maxSpeed = 50.0f;
    public bool canMove = true;
    public bool isFacingRight = true;

    [Space(10)]
    [Header("Jumping")]
    public float jumpForce = 10.0f;

    [Space(10)]
    [Header("Ground Check")]
    public bool isGrounded = false;
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundMask;

    [Space(10)]
    [Header("Pickup Check")]
    public bool isNearItem = false;
    public Transform pickupCheck;
    public float pickupCheckRadius;
    public LayerMask pickupMask;

    [Space(10)]
    [Header("Throwing")]
    public GameObject throwPowerIndicator;
    public bool isChargingThrow = false;
    public float throwChargeTime = 0f;
    public float maxThrowChargeTime = 2f; // Max time to reach full charge
    public float minThrowForce = 5f; // Minimum throw force
    public float maxThrowForce = 20f; // Maximum throw force based on max charge time

    [Space(10)]
    [Header("Inventory")]
    public GameObject itemToPickUp;
    public GameObject containerItem;
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

        throwPowerIndicator.SetActive(false);
        throwPowerIndicator.transform.localScale = new Vector3(0, throwPowerIndicator.transform.localScale.y, throwPowerIndicator.transform.localScale.z); // Hide the power indicator

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

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (containerItem != null)
            {
                // Interact with the container to take the item it's dispensing
                BagController bagController = containerItem.GetComponent<BagController>();
                if (bagController != null && carryingItem == null)
                {
                    takeItem(bagController.itemToDispense);
                }
            }
            else if (itemToPickUp != null && carryingItem == null)
            {
                // Pickup item from the ground
                pickupItem(itemToPickUp);
            }
            else if (carryingItem != null)
            {
                // Drop the currently carried item
                DropItem();
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
        CheckNearItem();
        Move(xInput);
        ApplyFriction();
    }

    void CheckInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        Debug.Log("xInput: " + xInput);

        if (xInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (xInput < 0 && isFacingRight)
        {
            Flip();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            gameManager.SwapTimePeriod();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            uiManager.TriggerTransition(0.5f, () =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            });
        }

        if (Input.GetKeyDown(KeyCode.F) && carryingItem != null)
        {
            StartChargingThrow();
        }
        else if (isChargingThrow && Input.GetKey(KeyCode.F))
        {
            // Continuously update charging if the 'F' key is held down
            UpdateChargingThrow();
        }
        else if (isChargingThrow && Input.GetKeyUp(KeyCode.F))
        {
            // Finish throwing when the 'F' key is released
            FinishThrowing();
        }
    }

    public void Move(float horizontal)
    {
        body.velocity = new Vector2(horizontal * speed, body.velocity.y);

        if ((horizontal > 0 && !isFacingRight) || (horizontal < 0 && isFacingRight))
        {
            Flip();
        }

        if (Mathf.Abs(body.velocity.x) > maxSpeed)
        {
            body.velocity = new Vector2(Mathf.Sign(body.velocity.x) * maxSpeed, body.velocity.y);
        }


        if (stopInAir && !isGrounded) // Player is not moving and in the air
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

    private void StartChargingThrow()
    {
        Debug.Log("Start charging throw: " + throwChargeTime);
        throwPowerIndicator.SetActive(true);
        isChargingThrow = true;
        throwChargeTime = 0f; // Reset charge time

        Rigidbody2D itemRb = carryingItem.GetComponent<Rigidbody2D>();
        if (itemRb != null)
        {
            itemRb.isKinematic = true;
            itemRb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    private void UpdateChargingThrow()
    {
        Debug.Log("Charging throw: " + throwChargeTime);
        throwPowerIndicator.transform.localScale = new Vector3(throwChargeTime / maxThrowChargeTime, throwPowerIndicator.transform.localScale.y, throwPowerIndicator.transform.localScale.z);
        throwChargeTime += Time.deltaTime;
        throwChargeTime = Mathf.Clamp(throwChargeTime, 0, maxThrowChargeTime);
    }

    private void FinishThrowing()
    {
        Debug.Log("Throwing item: " + throwChargeTime);
        isChargingThrow = false;
        throwPowerIndicator.transform.localScale = new Vector3(0, throwPowerIndicator.transform.localScale.y, throwPowerIndicator.transform.localScale.z); // Hide the power indicator
        throwPowerIndicator.SetActive(false);
        ThrowCarryingItem();
    }

    void ThrowCarryingItem()
    {
        if (carryingItem != null)
        {
            // Calculate the throw force based on the charge time
            float throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, throwChargeTime / maxThrowChargeTime);

            // Throw the item
            Vector2 throwDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

            // Apply the force to the carried item
            Rigidbody2D itemRb = carryingItem.GetComponent<Rigidbody2D>();
            if (itemRb != null)
            {
                itemRb.isKinematic = false;
                itemRb.constraints = RigidbodyConstraints2D.None;
            }
            itemRb.transform.SetParent(null); // Detach from the player
            itemRb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);
            carryingItem.GetComponent<Collider2D>().enabled = true;

            uiManager.ClearInventory(); // Update UI to remove the item
            carryingItem = null; // Clear the reference
        }

        throwChargeTime = 0f;
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask);
    }

    void CheckNearItem()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pickupCheck.position, pickupCheckRadius, pickupMask);
        GameObject closestPickupable = null;
        GameObject closestContainer = null;
        float closestPickupableDistance = float.MaxValue;
        float closestContainerDistance = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Seed") || hit.CompareTag("Pick-Up"))
            {
                float distance = Vector2.Distance(hit.transform.position, pickupCheck.position);
                if (distance < closestPickupableDistance)
                {
                    closestPickupable = hit.gameObject;
                    closestPickupableDistance = distance;
                }
            }
            else if (hit.CompareTag("Container"))
            {
                float distance = Vector2.Distance(hit.transform.position, pickupCheck.position);
                if (distance < closestContainerDistance)
                {
                    closestContainer = hit.gameObject;
                    closestContainerDistance = distance;
                }
            }
        }

        // Determine which item/container is closer and should be interacted with
        if (closestContainerDistance < closestPickupableDistance)
        {
            containerItem = closestContainer;
            itemToPickUp = null; // Ensure that itemToPickUp is cleared if a container is closer
        }
        else
        {
            itemToPickUp = closestPickupable;
            containerItem = null; // Clear the container reference if a pickupable item is closer
        }

        isNearItem = itemToPickUp != null || containerItem != null;
    }

    void ApplyFriction()
    {
        if (isGrounded && xInput == 0 && body.velocity.y <= 0f)
        {
            body.velocity *= groundDecay;
        }
    }

    public void takeItem(GameObject itemToTake)
    {
        if (itemToTake != null)
        {
            carryingItem = Instantiate(itemToTake, itemHoldPosition.position, Quaternion.identity, itemHoldPosition);
            carryingItem.GetComponent<Rigidbody2D>().isKinematic = true;
            carryingItem.GetComponent<Collider2D>().enabled = false;

            uiManager.UpdateInventory(carryingItem); // Assume this method updates the UI accordingly
            uiManager.HideHelpText();
        }
    }

    public void pickupItem(GameObject itemFromGround)
    {
        if (itemFromGround != null)
        {
            carryingItem = itemFromGround;
            itemFromGround.transform.position = itemHoldPosition.position;
            itemFromGround.transform.SetParent(itemHoldPosition);
            itemFromGround.GetComponent<Rigidbody2D>().isKinematic = true;
            if (itemFromGround.GetComponent<Collider2D>() != null)
            {
                itemFromGround.GetComponent<Collider2D>().enabled = false;
            }

            uiManager.UpdateInventory(carryingItem); // Update UI to show the picked-up item
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
        if (other.gameObject.CompareTag("Container") && carryingItem == null)
        {
            uiManager.ShowHelpText("Press \'E\' to Interact");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == itemToPickUp || other.gameObject == containerItem)
        {
            uiManager.HideHelpText(); // Hide interaction UI hints
        }

        if (other.gameObject.CompareTag("Container") || other.gameObject.CompareTag("Interactable"))
        {
            uiManager.HideHelpText();
            containerItem = null; // Clear the container reference if exiting container trigger
            itemToPickUp = null; // Clear the item reference if exiting item trigger
            isNearItem = false; // Reset the flag as there's no interactable item nearby
        }
    }
}
