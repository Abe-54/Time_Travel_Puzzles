using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSensor : MonoBehaviour
{
    public BoxCollider2D wallCheck;
    public LayerMask wallMask;

    public bool isNextToWall { get; private set; }

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
