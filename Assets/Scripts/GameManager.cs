using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

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

    public GameObject pastEnvironment;
    public GameObject presentEnvironment;

    public CinemachineVirtualCamera pastCamera;
    public CinemachineVirtualCamera presentCamera;

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
        pastEnvironment.SetActive(false);

        orignalGravity = player.GetComponent<Rigidbody2D>().gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SwapTimePeriod()
    {
        currentTimePeriod = (currentTimePeriod == TimePeriod.Past) ? TimePeriod.Present : TimePeriod.Past;

        // Freeze Everything for a moment to swap environments

        FreezeEverything();

        uiManager.TriggerTransition(1.0f, () =>
        {
            pastEnvironment.SetActive(currentTimePeriod == TimePeriod.Past);
            presentEnvironment.SetActive(currentTimePeriod == TimePeriod.Present);

            player.transform.position = (currentTimePeriod == TimePeriod.Past) ? pastSpawnPosition.position : presentSpawnPosition.position;

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
}
