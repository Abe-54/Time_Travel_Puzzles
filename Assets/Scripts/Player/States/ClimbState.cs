using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbState : BaseState
{
    public AnimationClip rightAnim;
    public AnimationClip leftAnim;

    public AnimationClip carryingItemRightAnim;
    public AnimationClip carryingItemLeftAnim;

    public float maxYSpeed;

    public float gravityScale;

    public override void EnterState()
    {
        // if (FindObjectOfType<PlayerMovement>().isCarryingItem)
        // {
        //     PlayDirectionalAnimation(carryingItemLeftAnim, carryingItemRightAnim);
        // }
        // else
        // {
        //     PlayDirectionalAnimation(leftAnim, rightAnim);
        // }

        Debug.Log("Entered Climb State");

        gravityScale = body.gravityScale;
        body.gravityScale = 0;
    }

    public override void Do()
    {
        float velY = body.velocity.y;
        animator.speed = Helpers.Map(Mathf.Abs(velY), 0, maxYSpeed, 0, 1f, true);

        if (!wallSensor.isNextToWall || body.velocity.y == 0)
        {
            isComplete = true;
        }
    }

    public override void FixedDo() { }

    public override void ExitState()
    {
        body.gravityScale = gravityScale;
    }
}
