using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    public GameObject tree;
    public GameObject knockedTree;

    public bool isKnocked;

    public bool isGrown;

    // Start is called before the first frame update
    void Start()
    {
        tree = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name.CompareTo("Axe") == 0)
        {
            isKnocked = true;

            Rigidbody2D axeBody = other.GetComponent<Rigidbody2D>();
            axeBody.bodyType = RigidbodyType2D.Static;
            axeBody.gravityScale = 0;
            axeBody.constraints = RigidbodyConstraints2D.FreezeAll;
            axeBody.velocity = Vector2.zero;
            axeBody.transform.SetParent(tree.transform);

            tree.transform.position = knockedTree.transform.position;
            tree.transform.rotation = knockedTree.transform.rotation;

            other.isTrigger = true;
            tree.GetComponent<Collider2D>().isTrigger = false;
        }
    }
}
