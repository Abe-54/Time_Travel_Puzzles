using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSwapState : BaseState
{
    public AnimationClip anim;

    public bool isTimeTraveling = false;
    public Vector2 playerVelocityBeforeSwap;
    public float originalGravity;
    public Transform watchPosition;

    public TimeSwapManager timeSwapManager;

    public override void EnterState()
    {
        isTimeTraveling = true;
        body.transform.localScale = new Vector3(body.transform.localScale.x, body.transform.localScale.y, 1);
        originalGravity = body.gravityScale;

        //Store Player Velocity
        playerVelocityBeforeSwap = body.velocity;

        //Freeze Player in place
        body.constraints = RigidbodyConstraints2D.FreezeAll;
        body.gravityScale = 0;
        body.velocity = Vector2.zero;
        body.angularVelocity = 0;

        Debug.Log("IN TIME SWAP STATE");

        animator.Play(anim.name);

        Debug.Log("Animator Playing: " + anim.name);
    }

    public override void Do()
    {
        animator.speed = 1;
    }

    public override void FixedDo() { }

    public override void ExitState()
    {

        Debug.Log("EXIT TIME SWAP STATE");

        isTimeTraveling = false;
        body.gravityScale = originalGravity;

        //Unfreeze Player
        body.constraints = RigidbodyConstraints2D.None;
        body.constraints = RigidbodyConstraints2D.FreezeRotation;

        //Restore Player Velocity
        body.velocity = playerVelocityBeforeSwap;
    }
}
