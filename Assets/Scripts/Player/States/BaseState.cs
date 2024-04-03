using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState : MonoBehaviour
{
    public bool isComplete { get; protected set; }

    protected float startTime;

    public float time => Time.time - startTime;

    protected Core core;

    protected Rigidbody2D body => core.body;
    protected Animator animator => core.animator;
    protected GroundSensor groundSensor => core.groundSensor;
    protected ItemSensor itemSensor => core.itemSensor;
    protected WallSensor wallSensor => core.wallSensor;

    public StateMachine machine;

    public StateMachine parent;

    public BaseState state => machine.state;

    protected void Set(BaseState newState, bool forceReset = false)
    {
        machine.Set(newState, forceReset);
    }

    public void SetCore(Core _core)
    {
        machine = new StateMachine();
        core = _core;
    }

    protected void PlayDirectionalAnimation(AnimationClip leftAnimation, AnimationClip rightAnimation, float normalizedTime = 1f, bool forceFlip = false)
    {
        AnimationClip animToPlay = core.transform.localScale.x > 0 ? rightAnimation : leftAnimation;
        animator.Play(animToPlay.name, 0, normalizedTime);

        // Optionally flip the sprite renderer if needed (forceFlip allows to override default behavior)
        if (forceFlip || core.GetComponent<SpriteRenderer>().flipX != (core.transform.localScale.x < 0))
        {
            core.GetComponent<SpriteRenderer>().flipX = core.transform.localScale.x < 0;
        }
    }

    public virtual void EnterState() { }
    public virtual void Do() { }
    public virtual void FixedDo() { }
    public virtual void ExitState() { }

    public void DoBranch()
    {
        Do();
        state?.DoBranch();
    }

    public void FixedDoBranch()
    {
        FixedDo();
        state?.FixedDoBranch();
    }

    public void Initialize(StateMachine _parent)
    {
        parent = _parent;
        isComplete = false;
        startTime = Time.time;
    }
}
