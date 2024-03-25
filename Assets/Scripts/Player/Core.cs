using Unity.VisualScripting;
using UnityEngine;

public abstract class Core : MonoBehaviour
{
    [Header("Core Components")]
    public Rigidbody2D body;
    public Animator animator;
    public GroundSensor groundSensor;
    public GameObject carryingItem;

    public StateMachine machine;

    public void SetupInstances()
    {
        machine = new StateMachine();

        BaseState[] states = GetComponentsInChildren<BaseState>();
        foreach (BaseState state in states)
        {
            state.SetCore(this);
        }
    }
}