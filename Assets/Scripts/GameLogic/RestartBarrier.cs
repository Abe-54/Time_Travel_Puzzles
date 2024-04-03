using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartBarrier : MonoBehaviour
{
    public List<GameObject> necessaryObjects;

    private UIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (necessaryObjects.Contains(other.gameObject))
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
