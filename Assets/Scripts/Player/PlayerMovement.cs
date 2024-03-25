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

    [Header("Components")]
    //movement properties
    public float acceleration;
    [Range(0f, 1f)]
    public float groundDecay;
    public bool isCarryingItem = false;

    //variables
    public float xInput { get; private set; }
    public float yInput { get; private set; }
    public bool pickupInput { get; private set; }

    private Vector3 originalScale;

    void Start()
    {
        SetupInstances();
        machine.Set(idleState);

        originalScale = transform.localScale;
    }


    // Update is called once per frame
    void Update()
    {
        CheckInput();
        HandleJump();

        SelectState();

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

    private void HandleItemInteraction()
    {
        if (pickupInput)
        {
            if (itemSensor.itemDetected && !isCarryingItem)
            {
                Debug.Log("Picking up item");
                PickupItem(itemSensor.item);
            }
            else if (isCarryingItem)
            {
                Debug.Log("Dropping item");
                DropItem();
            }
            machine.Set(idleState, true);
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
            // Additional logic for UI update or effects can be added here
        }
    }

    private void DropItem()
    {
        if (carryingItem != null)
        {
            // Logic to drop the item
            carryingItem.transform.SetParent(null); // Detach the item from the player
            carryingItem.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            carryingItem.GetComponent<Rigidbody2D>().isKinematic = false;
            carryingItem.GetComponent<Collider2D>().enabled = true;
            isCarryingItem = false;
            carryingItem = null;
            // Additional logic for UI update or effects can be added here
        }
    }
}
