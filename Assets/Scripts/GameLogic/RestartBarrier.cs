using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartBarrier : MonoBehaviour
{
    public List<GameObject> necessaryObjects;

    private UIManager uiManager;
    private PlayerMovement player;

    // Start is called before the first frame update
    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        player = FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (necessaryObjects.Contains(other.gameObject) || other.gameObject == player.gameObject)
        {
            Rigidbody2D body = other.GetComponent<Rigidbody2D>();

            if (body != null)
            {
                body.constraints = RigidbodyConstraints2D.FreezeAll;
            }

            uiManager.ShowHelpText("Press \'R\' to Restart Level");
        }
    }
}
