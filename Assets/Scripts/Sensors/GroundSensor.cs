using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSensor : MonoBehaviour
{
    public BoxCollider2D groundCheck;
    public LayerMask groundMask;

    public bool grounded { get; private set; }

    void FixedUpdate()
    {
        CheckGround();
    }

    void CheckGround()
    {
        grounded = Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.bounds.center, groundCheck.bounds.size);
    }
}
