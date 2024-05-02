using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : Core
{
    [Space(20)]
    [Header("State Machine")]
    public IdleState idleState;
    public RunState runningState;
    public AirState airState;
    public TimeSwapState timeSwapState;
    public ClimbState climbState;

    [Header("Components")]
    //movement properties
    public AudioSource climbAudio;
    public float acceleration;
    [Range(0f, 1f)]
    public float groundDecay;
    public bool isCarryingItem = false;

    [Header("Throwing")]
    public GameObject throwPowerIndicator;
    public bool isChargingThrow = false;
    public float throwChargeTime = 0f;
    public float maxThrowChargeTime = 2f; // Max time to reach full charge
    public float minThrowForce = 5f; // Minimum throw force
    public float maxThrowForce = 20f; // Maximum throw force based on max charge time

    [Header("Coyote Time")]
    public float coyoteTime = 0.1f;
    public float coyoteTimer = 0f;

    //variables
    public float xInput { get; private set; }
    public float yInput { get; private set; }
    public bool pickupInput { get; private set; }
    public bool restartInput { get; private set; }

    private Vector3 originalScale;
    private UIManager uiManager;
    private TimeSwapManager timeSwapManager;

    void Start()
    {
        SetupInstances();
        machine.Set(idleState);

        originalScale = transform.localScale;
        uiManager = FindObjectOfType<UIManager>();
        timeSwapManager = FindObjectOfType<TimeSwapManager>();
    }


    // Update is called once per frame
    void Update()
    {
        CheckInput();
        UpdateCoyoteTime();
        HandleJump();
        HandleThrowing();
        HandleTimeTravel();

        if (!timeSwapState.isTimeTraveling)
        { SelectState(); }

        if (isCarryingItem && carryingItem != null)
        {
            uiManager.ShowThrowPrompt();
        }
        else
        {
            uiManager.HideThrowPrompt();
        }

        machine.state.DoBranch();

        HandleItemInteraction();
    }

    void FixedUpdate()
    {
        ApplyFriction();
        HandleXMovement();
        HandleClimb();
    }

    void UpdateCoyoteTime()
    {
        // If grounded, reset the coyote timer, otherwise increase it
        if (groundSensor.grounded)
        {
            coyoteTimer = 0f;
        }
        else
        {
            coyoteTimer += Time.deltaTime;
        }
    }

    void SelectState()
    {

        if (restartInput)
        {
            machine.Set(idleState);

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (groundSensor.grounded)
        {
            if (xInput == 0)
            {
                machine.Set(idleState);
            }
            else
            {
                machine.Set(runningState);
            }
        }
        else if (wallSensor.isNextToWall)
        {
            if (yInput == 0)
            {
                Debug.Log("In air state");
                machine.Set(airState);
            }
            else
            {
                Debug.Log("In wall climb state");
                machine.Set(climbState);
            }
        }
        else
        {
            machine.Set(airState);
        }
    }

    void CheckInput()
    {
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");
        pickupInput = Input.GetKeyDown(KeyCode.E);
        restartInput = Input.GetKeyDown(KeyCode.R);
    }

    void HandleXMovement()
    {
        if (Mathf.Abs(xInput) > 0)
        {
            body.velocity = new Vector2(xInput * runningState.maxXSpeed, body.velocity.y);

            FaceInput();
        }
    }

    void HandleClimb()
    {
        if (yInput != 0 && wallSensor.isNextToWall)
        {
            Debug.Log("Climbing");
            body.velocity = new Vector2(body.velocity.x, yInput * climbState.maxYSpeed);

            if (!climbAudio.isPlaying)
            {
                climbAudio.PlayOneShot(climbAudio.clip);
            }
        }
        else
        {
            climbAudio.Stop();
        }
    }

    void FaceInput()
    {
        float direction = Mathf.Sign(xInput);
        transform.localScale = new Vector3(direction * originalScale.x, originalScale.y, originalScale.z);
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && (groundSensor.grounded || coyoteTimer <= coyoteTime))
        {
            body.velocity = new Vector2(body.velocity.x, airState.jumpSpeed);
            coyoteTimer = coyoteTime + 1;
        }
    }

    void HandleThrowing()
    {
        if (Input.GetKeyDown(KeyCode.F) && carryingItem != null)
        {
            uiManager.HideThrowPrompt();
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

    void HandleTimeTravel()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !timeSwapState.isTimeTraveling)
        {
            timeSwapManager.SwapTimePeriod();
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
            isCarryingItem = false; // Update the carrying state
        }

        throwChargeTime = 0f;

        machine.Set(idleState, true);
    }

    private void HandleItemInteraction()
    {
        if (!pickupInput) return;

        // Try dropping the item if carrying one.
        if (isCarryingItem)
        {
            DropItem();
            return;
        }

        // Early exit if no item detected.
        if (!itemSensor.itemDetected) return;

        GameObject detectedItem = itemSensor.item;
        IInteractable interactable = detectedItem.GetComponent<IInteractable>();

        // Interact using the IInteractable interface if available.
        if (interactable != null)
        {
            interactable.Interact(this);
            return;
        }

        // Fallback to tag-based interaction for simple pickup items.
        if (detectedItem.CompareTag("Pick-Up"))
        {
            PickupItem(detectedItem);
        }
    }


    void ApplyFriction()
    {
        // Stop player if not moving
        if (groundSensor.grounded && xInput == 0)
        {
            body.velocity = new Vector2(0, body.velocity.y);
        }
    }

    public void PickupItem(GameObject item)
    {
        if (item != null)
        {
            uiManager.HidePickUpPrompt();
            uiManager.ShowThrowPrompt();

            carryingItem = item;
            item.transform.position = itemHoldPosition.position;
            item.transform.SetParent(itemHoldPosition);
            carryingItem.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            carryingItem.GetComponent<Rigidbody2D>().isKinematic = true;
            carryingItem.GetComponent<Collider2D>().enabled = false;
            isCarryingItem = true;

            uiManager.UpdateInventory(carryingItem); // Update UI to show the item
        }

        machine.Set(idleState, true);
    }

    private void DropItem()
    {
        if (carryingItem != null)
        {
            // Logic to drop the item
            carryingItem.transform.SetParent(null); // Detach the item from the player

            carryingItem.transform.position = new Vector3(itemSensor.itemCheck.transform.position.x, itemSensor.itemCheck.transform.position.y, itemSensor.itemCheck.transform.position.z); // Drop the item in front of the player 

            carryingItem.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            carryingItem.GetComponent<Rigidbody2D>().isKinematic = false;
            carryingItem.GetComponent<Collider2D>().enabled = true;
            isCarryingItem = false;
            carryingItem = null;

            // Additional logic for UI update or effects can be added here

            uiManager.ClearInventory(); // Update UI to remove the item
        }

        machine.Set(idleState, true);
    }
}
