using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PastState : TimeSwapBaseState
{
    public GameObject[] pastObjects;
    public Sprite pastBackground;

    public override void EnterState()
    {
        backgroundSprite.sprite = pastBackground;

        foreach (GameObject gameObject in pastObjects)
        {
            gameObject.SetActive(true);
        }
    }

    public override void Do()
    {
    }

    public override void ExitState()
    {
        foreach (GameObject gameObject in pastObjects)
        {
            gameObject.SetActive(false);
        }
    }
}
