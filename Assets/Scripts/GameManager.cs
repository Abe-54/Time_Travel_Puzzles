using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class GameManager : MonoBehaviour
{
    public enum TimePeriod
    {
        Past,
        Present
    }

    public TimePeriod currentTimePeriod = TimePeriod.Present;

    public Transform pastSpawnPosition;
    public Transform presentSpawnPosition;

    public GameObject[] pastObjects;

    public GameObject[] presentObjects;

    public CinemachineVirtualCamera pastCamera;
    public CinemachineVirtualCamera presentCamera;

    public event Action OnTimePeriodChanged;

    // Temporary Hardcoded puzzle solution for the demo
    public DirtController dirt;
    public GameObject tree;
    public GameObject knockedTree;

    private PlayerController player;
    private UIManager uiManager;
    private float orignalGravity;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        uiManager = FindObjectOfType<UIManager>();

        currentTimePeriod = TimePeriod.Present;
        toggleGameobjects(pastObjects, false);
        orignalGravity = player.GetComponent<Rigidbody2D>().gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SwapTimePeriod()
    {
        currentTimePeriod = (currentTimePeriod == TimePeriod.Past) ? TimePeriod.Present : TimePeriod.Past;

        Vector2 playerVelocityBeforeSwap = player.GetComponent<Rigidbody2D>().velocity;

        // Freeze Everything for a moment to swap environments
        FreezeEverything();

        uiManager.TriggerTransition(1.0f, () =>
        {
            OnTimePeriodChanged?.Invoke();

            toggleGameobjects(pastObjects, currentTimePeriod == TimePeriod.Past);
            toggleGameobjects(presentObjects, currentTimePeriod == TimePeriod.Present);

            if (currentTimePeriod == TimePeriod.Past)
            {
                pastCamera.Priority = 11; // Higher priority takes over
                presentCamera.Priority = 10;
            }
            else
            {
                pastCamera.Priority = 10;
                presentCamera.Priority = 11; // Higher priority takes over
            }

            player.GetComponent<Rigidbody2D>().velocity = playerVelocityBeforeSwap;

            UnfreezeEverything();
        });
    }

    void FreezeEverything()
    {
        // Freeze all enemies, platforms, etc.

        player.isGrounded = false;
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        player.GetComponent<Rigidbody2D>().gravityScale = 0;
        player.canMove = false;
    }

    void UnfreezeEverything()
    {
        // Unfreeze all enemies, platforms, etc.

        player.GetComponent<Rigidbody2D>().gravityScale = orignalGravity;
        player.canMove = true;
    }

    void toggleGameobjects(GameObject[] gameObjects, bool state)
    {
        foreach (GameObject gameObject in gameObjects)
        {
            gameObject.SetActive(state);
        }
    }
}
