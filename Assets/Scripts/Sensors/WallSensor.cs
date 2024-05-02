using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSensor : MonoBehaviour
{
    public BoxCollider2D wallCheck;
    public LayerMask wallMask;

    private UIManager uiManager;

    private void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    public bool isNextToWall { get; private set; }

    private void Update()
    {
        if (isNextToWall)
        {
            uiManager.ShowClimbingPrompt();
        }
        else
        {
            uiManager.HideClimbingPrompt();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckWall();
    }

    void CheckWall()
    {
        isNextToWall = Physics2D.OverlapAreaAll(wallCheck.bounds.min, wallCheck.bounds.max, wallMask).Length > 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheck.bounds.center, wallCheck.bounds.size);
    }
}
