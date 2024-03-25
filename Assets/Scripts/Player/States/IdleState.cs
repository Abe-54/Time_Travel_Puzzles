using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState
{
    public AnimationClip rightAnim;
    public AnimationClip leftAnim;

    public override void EnterState()
    {
        PlayDirectionalAnimation(leftAnim, rightAnim);
    }

    public override void Do()
    {
        if (!groundSensor.grounded)
        {
            isComplete = true;
        }
    }

    public override void FixedDo() { }

    public override void ExitState() { }
}
