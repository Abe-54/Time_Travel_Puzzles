using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TimeSwapBaseState : MonoBehaviour
{
    public bool isComplete { get; protected set; }

    protected float startTime;

    public float time => Time.time - startTime;

    // Variables for the TimeSwapBaseState
    protected GameManager gameManager;
    protected PlayerController player;
    protected SpriteRenderer backgroundSprite;

    public virtual void EnterState() { }

    public virtual void Do() { }

    public virtual void FixedDo() { }

    public virtual void ExitState() { }

    public void SetupState(GameManager gameManager, PlayerController player, SpriteRenderer backgroundSprite)
    {
        this.gameManager = gameManager;
        this.player = player;
        this.backgroundSprite = backgroundSprite;
    }

    public void Initialize()
    {
        isComplete = false;
        startTime = Time.time;
    }

    public void FreezeEverything()
    {
        // Freeze all enemies, platforms, etc.

        player.isGrounded = false;
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        player.GetComponent<Rigidbody2D>().gravityScale = 0;
        player.canMove = false;
    }

    public void UnfreezeEverything()
    {
        // Unfreeze all enemies, platforms, etc.
        player.GetComponent<Rigidbody2D>().gravityScale = gameManager.orignalGravity;
        player.canMove = true;
    }
}
