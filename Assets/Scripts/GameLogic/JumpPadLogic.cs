using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPadLogic : MonoBehaviour
{
    public float jumpForce = 10.0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Rigidbody2D>().velocity = new Vector2(other.GetComponent<Rigidbody2D>().velocity.x, jumpForce);
        }
    }
}
