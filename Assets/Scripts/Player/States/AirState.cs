using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirState : BaseState
{
    public AnimationClip rightAnim;
    public AnimationClip leftAnim;
    public float jumpSpeed;

    public override void EnterState()
    {
        PlayDirectionalAnimation(leftAnim, rightAnim);
    }

    public override void Do()
    {
        float time = Helpers.Map(body.velocity.y, jumpSpeed, -jumpSpeed, 0, 1, true);
        PlayDirectionalAnimation(leftAnim, rightAnim, time);
        animator.speed = 0;

        if (groundSensor.grounded)
        {
            isComplete = true;
        }
    }

    public override void FixedDo() { }

    public override void ExitState() { }
}
