using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirState : BaseState
{
    public AnimationClip rightAnim;
    public AnimationClip leftAnim;

    public AnimationClip carryingItemRightAnim;
    public AnimationClip carryingItemLeftAnim;

    public float jumpSpeed;

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
        float time = Helpers.Map(body.velocity.y, jumpSpeed, -jumpSpeed, 0, 1, true);
        PlayDirectionalAnimation(FindObjectOfType<PlayerMovement>().isCarryingItem ? carryingItemLeftAnim : leftAnim, FindObjectOfType<PlayerMovement>().isCarryingItem ? carryingItemRightAnim : rightAnim, time);
        animator.speed = 0;

        if (groundSensor.grounded)
        {
            isComplete = true;
        }
    }

    public override void FixedDo() { }

    public override void ExitState() { }
}
