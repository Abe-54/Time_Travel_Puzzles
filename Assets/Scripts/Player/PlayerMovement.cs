using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Core
{
    [Space(20)]
    [Header("State Machine")]
    public IdleState idleState;
    public RunState runningState;
    public AirState airState;
    public TimeSwapState timeSwapState;

    [Header("Components")]
    //movement properties
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

    //variables
    public float xInput { get; private set; }
    public float yInput { get; private set; }
    public bool pickupInput { get; private set; }

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
        HandleJump();
        HandleThrowing();
        HandleTimeTravel();

        if (!timeSwapState.isTimeTraveling)
        { SelectState(); }

        machine.state.DoBranch();

        HandleItemInteraction();
    }

    void FixedUpdate()
    {

        HandleXMovement();
        ApplyFriction();
    }

    void SelectState()
    {
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
    }

    void HandleXMovement()
    {
        if (Mathf.Abs(xInput) > 0)
        {
            //increment velocity by our accelleration, then clamp within max
            float newSpeed = xInput * runningState.maxXSpeed;

            body.velocity = new Vector2(newSpeed, body.velocity.y);

            FaceInput();
        }
    }

    void FaceInput()
    {
        float direction = Mathf.Sign(xInput);
        transform.localScale = new Vector3(direction * originalScale.x, originalScale.y, originalScale.z);
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && groundSensor.grounded)
        {
            body.velocity = new Vector2(body.velocity.x, airState.jumpSpeed);
        }
    }

    void HandleThrowing()
    {
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
        if (pickupInput)
        {
            if (itemSensor.itemDetected && !isCarryingItem)
            {
                if (itemSensor.item.CompareTag("Container"))
                {
                    Debug.Log("Taking item from container");
                    GameObject item = itemSensor.item.GetComponent<BagController>().SpawnItem(itemHoldPosition.gameObject);
                    PickupItem(item);
                }
                else
                {
                    Debug.Log("Picking up item");
                    PickupItem(itemSensor.item);
                }
            }
            else if (isCarryingItem)
            {
                Debug.Log("Dropping item");
                DropItem();
            }
        }
    }


    void ApplyFriction()
    {
        if (groundSensor.grounded && xInput == 0 && body.velocity.y <= 0)
        {
            body.velocity *= groundDecay;
        }
    }

    private void PickupItem(GameObject item)
    {
        if (item != null)
        {
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
