using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : BaseState
{
    public AnimationClip rightAnim;
    public AnimationClip leftAnim;
    public AnimationClip carryingItemRightAnim;
    public AnimationClip carryingItemLeftAnim;
    public float maxXSpeed;

    public override void EnterState()
    {
        if (FindObjectOfType<PlayerMovement>().isCarryingItem)
        {
            PlayDirectionalAnimation(carryingItemLeftAnim, carryingItemRightAnim);
        }
        else
        {
            PlayDirectionalAnimation(leftAnim, rightAnim);
        }
    }

    public override void Do()
    {
        float velX = body.velocity.x;
        animator.speed = Helpers.Map(Mathf.Abs(velX), 0, maxXSpeed, 0, 1f, true);

        if (!groundSensor.grounded)
        {
            isComplete = true;
        }
    }

    public override void FixedDo() { }

    public override void ExitState() { }
}
