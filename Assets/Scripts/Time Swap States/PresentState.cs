using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresentState : TimeSwapBaseState
{
    public GameObject[] presentObjects;
    public Sprite presentBackground;

    public override void EnterState()
    {
        backgroundSprite.sprite = presentBackground;

        foreach (GameObject gameObject in presentObjects)
        {
            gameObject.SetActive(true);
        }
    }

    public override void Do()
    {
    }

    public override void ExitState()
    {
        foreach (GameObject gameObject in presentObjects)
        {
            gameObject.SetActive(false);
        }
    }
}
