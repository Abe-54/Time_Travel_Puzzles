using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirtController : MonoBehaviour
{
    public bool hasSeed = false;
    public GameObject seedBag;

    private SpriteRenderer spriteRenderer;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasSeed)
        {
            spriteRenderer.color = new Color(0, 1, 0, 1);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Seed"))
        {
            Debug.Log("Seed has been planted");
            hasSeed = true;
            gameManager.tree.SetActive(true);
            seedBag.SetActive(false);
            Destroy(other.gameObject);
        }
    }

}
