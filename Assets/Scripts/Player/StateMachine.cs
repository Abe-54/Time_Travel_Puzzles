using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public BaseState state;

    public void Set(BaseState newState, bool forceReset = false)
    {
        if (state != newState || forceReset)
        {
            state?.ExitState();
            state = newState;
            state.Initialize();
            state.EnterState();
        }
    }
}
