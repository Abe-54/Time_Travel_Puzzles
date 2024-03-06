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

    public GameObject[] pastObjects;

    public GameObject[] presentObjects;
    public event Action OnTimePeriodChanged;

    private PlayerController player;
    private UIManager uiManager;
    private float orignalGravity;

    public SpriteRenderer backgroundSprite;
    public Sprite[] backgroundSprites = new Sprite[2];

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        uiManager = FindObjectOfType<UIManager>();

        currentTimePeriod = TimePeriod.Present;
        backgroundSprite.sprite = backgroundSprites[0];

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

        player.isTimeTraveling = true;

        // Freeze Everything for a moment to swap environments
        FreezeEverything();

        uiManager.TriggerTransition(1.0f, () =>
        {
            backgroundSprite.sprite = (currentTimePeriod == TimePeriod.Present) ? backgroundSprites[0] : backgroundSprites[1];

            OnTimePeriodChanged?.Invoke();

            toggleGameobjects(pastObjects, currentTimePeriod == TimePeriod.Past);
            toggleGameobjects(presentObjects, currentTimePeriod == TimePeriod.Present);

            player.isTimeTraveling = false;

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
